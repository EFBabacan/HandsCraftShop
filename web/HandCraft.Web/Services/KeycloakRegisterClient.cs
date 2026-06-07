using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HandCraft.Web.Services
{
    // Public site icin musteri kaydi: Keycloak Admin API ile 'customer' rollu kullanici olusturur.
    public class KeycloakRegisterClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;

        public KeycloakRegisterClient(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        private string BaseUrl => _cfg["Keycloak:BaseUrl"] ?? "http://localhost:8080";
        private string Realm => _cfg["Keycloak:Realm"] ?? "handcraft";
        private string AdminUser => _cfg["Keycloak:AdminUser"] ?? "admin";
        private string AdminPass => _cfg["Keycloak:AdminPassword"] ?? "admin";
        private string AdminBase => $"{BaseUrl}/admin/realms/{Realm}";

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

        // Yeni musteri olustur. Donus: (basarili, hata mesaji)
        public async Task<(bool ok, string? hata)> MusteriKaydetAsync(
            string kullaniciAdi, string email, string ad, string soyad, string parola)
        {
            var token = await TokenAlAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new
            {
                username = kullaniciAdi,
                email = email,
                firstName = ad,
                lastName = soyad,
                enabled = true,
                emailVerified = true,
                credentials = new[] { new { type = "password", value = parola, temporary = false } }
            };

            var resp = await _http.PostAsJsonAsync($"{AdminBase}/users", body);
            if (resp.StatusCode == System.Net.HttpStatusCode.Conflict)
                return (false, "Bu kullanici adi veya e-posta zaten kayitli.");
            if (!resp.IsSuccessStatusCode)
                return (false, $"Kayit basarisiz ({(int)resp.StatusCode}).");

            // Olusan kullaniciya 'customer' rolu ata
            var loc = resp.Headers.Location?.ToString();
            var userId = loc?.TrimEnd('/').Split('/').Last();
            if (string.IsNullOrEmpty(userId))
            {
                var bulunan = await _http.GetFromJsonAsync<List<JsonElement>>(
                    $"{AdminBase}/users?username={Uri.EscapeDataString(kullaniciAdi)}&exact=true");
                if (bulunan is { Count: > 0 })
                    userId = bulunan[0].GetProperty("id").GetString();
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var rol = await _http.GetFromJsonAsync<JsonElement>($"{AdminBase}/roles/customer");
                var atama = new[] { new { id = rol.GetProperty("id").GetString(), name = "customer" } };
                await _http.PostAsJsonAsync($"{AdminBase}/users/{userId}/role-mappings/realm", atama);

                // E-posta dogrulama maili gonder (varsa). Mailhog'a duser.
                if (!string.IsNullOrWhiteSpace(email))
                {
                    try
                    {
                        var req = new HttpRequestMessage(HttpMethod.Put,
                            $"{AdminBase}/users/{userId}/execute-actions-email")
                        { Content = JsonContent.Create(new[] { "VERIFY_EMAIL" }) };
                        await _http.SendAsync(req);
                    }
                    catch { /* mail gonderilemese de kayit basarili sayilir */ }
                }
            }

            return (true, null);
        }

        // ---- Profil (kullanici kendi bilgisini yonetir; userId token 'sub'dan gelir) ----
        public async Task<(string ad, string soyad, string email)> ProfilGetirAsync(string userId)
        {
            var token = await TokenAlAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var u = await _http.GetFromJsonAsync<JsonElement>($"{AdminBase}/users/{userId}");
            string G(string k) => u.TryGetProperty(k, out var v) ? (v.GetString() ?? "") : "";
            return (G("firstName"), G("lastName"), G("email"));
        }

        public async Task ProfilGuncelleAsync(string userId, string ad, string soyad, string email)
        {
            var token = await TokenAlAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var body = new { firstName = ad, lastName = soyad, email = email };
            await _http.PutAsJsonAsync($"{AdminBase}/users/{userId}", body);
        }

        public async Task SifreDegistirAsync(string userId, string yeniParola)
        {
            var token = await TokenAlAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var cred = new { type = "password", value = yeniParola, temporary = false };
            await _http.PutAsJsonAsync($"{AdminBase}/users/{userId}/reset-password", cred);
        }
    }
}
