using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HandCraft.Admin.Models;

namespace HandCraft.Admin.Services
{
    // Keycloak Admin REST API ile kullanici yonetimi (liste/ekle/duzenle/sil + rol atama).
    // Master realm admin hesabiyla token alir, handcraft realm'inde islem yapar.
    public class KeycloakAdminClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;

        public KeycloakAdminClient(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        private string BaseUrl => _cfg["Keycloak:BaseUrl"] ?? "http://localhost:8080";
        private string Realm => _cfg["Keycloak:Realm"] ?? "handcraft";
        private string AdminUser => _cfg["Keycloak:AdminUser"] ?? "admin";
        private string AdminPass => _cfg["Keycloak:AdminPassword"] ?? "admin";

        // master realm admin token
        private async Task<string> TokenAlAsync()
        {
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "admin-cli",
                ["username"] = AdminUser,
                ["password"] = AdminPass
            });
            var resp = await _http.PostAsync($"{BaseUrl}/realms/master/protocol/openid-connect/token", form);
            resp.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("access_token").GetString()!;
        }

        private async Task EkleBearerAsync()
        {
            var token = await TokenAlAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private string AdminBase => $"{BaseUrl}/admin/realms/{Realm}";

        // ---- Listeleme ----
        public async Task<List<KullaniciVm>> KullanicilariGetirAsync()
        {
            await EkleBearerAsync();
            var users = await _http.GetFromJsonAsync<List<KeycloakUser>>($"{AdminBase}/users?max=200") ?? new();
            var liste = new List<KullaniciVm>();
            foreach (var u in users)
            {
                var roller = await RolleriGetirAsync(u.Id);
                liste.Add(new KullaniciVm
                {
                    Id = u.Id, KullaniciAdi = u.Username ?? "", Email = u.Email ?? "",
                    Ad = u.FirstName ?? "", Soyad = u.LastName ?? "",
                    Aktif = u.Enabled, Roller = roller
                });
            }
            return liste;
        }

        private async Task<List<string>> RolleriGetirAsync(string userId)
        {
            var roller = await _http.GetFromJsonAsync<List<KeycloakRole>>(
                $"{AdminBase}/users/{userId}/role-mappings/realm") ?? new();
            return roller.Select(r => r.Name).ToList();
        }

        public async Task<KullaniciVm?> KullaniciGetirAsync(string id)
        {
            await EkleBearerAsync();
            var u = await _http.GetFromJsonAsync<KeycloakUser>($"{AdminBase}/users/{id}");
            if (u is null) return null;
            return new KullaniciVm
            {
                Id = u.Id, KullaniciAdi = u.Username ?? "", Email = u.Email ?? "",
                Ad = u.FirstName ?? "", Soyad = u.LastName ?? "",
                Aktif = u.Enabled, Roller = await RolleriGetirAsync(u.Id)
            };
        }

        // ---- Ekleme (rol + parola ile) ----
        public async Task<(bool ok, string? hata)> KullaniciEkleAsync(KullaniciFormVm form)
        {
            await EkleBearerAsync();
            var body = new
            {
                username = form.KullaniciAdi,
                email = form.Email,
                firstName = form.Ad,
                lastName = form.Soyad,
                enabled = true,
                emailVerified = true,
                credentials = new[] { new { type = "password", value = form.Parola, temporary = false } }
            };
            var resp = await _http.PostAsJsonAsync($"{AdminBase}/users", body);
            if (!resp.IsSuccessStatusCode)
                return (false, $"Kullanici olusturulamadi ({(int)resp.StatusCode}).");

            // Olusan kullanicinin id'sini Location header'dan al
            var loc = resp.Headers.Location?.ToString();
            var userId = loc?.TrimEnd('/').Split('/').Last();
            if (string.IsNullOrEmpty(userId))
            {
                // fallback: username ile bul
                var bulunan = await _http.GetFromJsonAsync<List<KeycloakUser>>($"{AdminBase}/users?username={form.KullaniciAdi}&exact=true");
                userId = bulunan?.FirstOrDefault()?.Id;
            }
            if (!string.IsNullOrEmpty(userId) && form.Roller.Count > 0)
                await RolAtaAsync(userId, form.Roller);

            return (true, null);
        }

        // ---- Guncelleme ----
        public async Task GuncelleAsync(KullaniciFormVm form)
        {
            await EkleBearerAsync();
            var body = new
            {
                email = form.Email, firstName = form.Ad, lastName = form.Soyad, enabled = form.Aktif
            };
            await _http.PutAsJsonAsync($"{AdminBase}/users/{form.Id}", body);

            // Parola verildiyse sifirla
            if (!string.IsNullOrWhiteSpace(form.Parola))
            {
                var cred = new { type = "password", value = form.Parola, temporary = false };
                await _http.PutAsJsonAsync($"{AdminBase}/users/{form.Id}/reset-password", cred);
            }

            // Rolleri senkronize et (once mevcutlari sil, sonra yenileri ata)
            await RolleriSifirlaVeAtaAsync(form.Id, form.Roller);
        }

        // ---- Silme ----
        public async Task SilAsync(string id)
        {
            await EkleBearerAsync();
            await _http.DeleteAsync($"{AdminBase}/users/{id}");
        }

        // ---- Rol yardimcilari ----
        private async Task<KeycloakRole?> RealmRolGetirAsync(string rolAdi)
        {
            return await _http.GetFromJsonAsync<KeycloakRole>($"{AdminBase}/roles/{rolAdi}");
        }

        private async Task RolAtaAsync(string userId, List<string> roller)
        {
            var atanacak = new List<KeycloakRole>();
            foreach (var r in roller)
            {
                var rol = await RealmRolGetirAsync(r);
                if (rol is not null) atanacak.Add(rol);
            }
            if (atanacak.Count > 0)
                await _http.PostAsJsonAsync($"{AdminBase}/users/{userId}/role-mappings/realm", atanacak);
        }

        private async Task RolleriSifirlaVeAtaAsync(string userId, List<string> yeniRoller)
        {
            // mevcut realm rollerini al
            var mevcut = await _http.GetFromJsonAsync<List<KeycloakRole>>(
                $"{AdminBase}/users/{userId}/role-mappings/realm") ?? new();
            // sadece customer/admin ile ilgilen (default rolleri bozma)
            var yonetilen = new[] { "customer", "admin" };
            var silinecek = mevcut.Where(m => yonetilen.Contains(m.Name) && !yeniRoller.Contains(m.Name)).ToList();
            if (silinecek.Count > 0)
            {
                var req = new HttpRequestMessage(HttpMethod.Delete, $"{AdminBase}/users/{userId}/role-mappings/realm")
                { Content = JsonContent.Create(silinecek) };
                await _http.SendAsync(req);
            }
            var eklenecek = yeniRoller.Where(r => !mevcut.Any(m => m.Name == r)).ToList();
            if (eklenecek.Count > 0)
                await RolAtaAsync(userId, eklenecek);
        }

        // Keycloak JSON modelleri
        private class KeycloakUser
        {
            public string Id { get; set; } = "";
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public bool Enabled { get; set; }
        }

        private class KeycloakRole
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
        }
    }
}
