using System.Net.Http.Headers;
using System.Net.Http.Json;
using HandCraft.Admin.Models;
using Microsoft.AspNetCore.Authentication;

namespace HandCraft.Admin.Services
{
    // Tum yonetim cagrilarini Gateway (7000) uzerinden, admin token'i Bearer ile yapar.
    public class AdminGatewayClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminGatewayClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task BearerEkleAsync()
        {
            var ctx = _httpContextAccessor.HttpContext;
            var token = ctx is null ? null : await ctx.GetTokenAsync("access_token");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // ---- Katalog: Urun ----
        public async Task<List<UrunVm>> UrunleriGetirAsync()
        {
            var s = await _http.GetFromJsonAsync<ApiSonuc<List<UrunVm>>>("/katalog/api/urun");
            return s?.Data ?? new();
        }

        public async Task<UrunVm?> UrunGetirAsync(string id)
        {
            var s = await _http.GetFromJsonAsync<ApiSonuc<UrunVm>>($"/katalog/api/urun/{id}");
            return s?.Data;
        }

        public async Task UrunEkleAsync(UrunFormVm dto)
        {
            await BearerEkleAsync();
            await _http.PostAsJsonAsync("/katalog/api/urun", new
            {
                dto.Ad, dto.Aciklama, dto.Fiyat, dto.KategoriId, dto.ImageUrl, dto.Stok
            });
        }

        public async Task UrunGuncelleAsync(UrunFormVm dto)
        {
            await BearerEkleAsync();
            await _http.PutAsJsonAsync("/katalog/api/urun", dto);
        }

        public async Task UrunSilAsync(string id)
        {
            await BearerEkleAsync();
            await _http.DeleteAsync($"/katalog/api/urun/{id}");
        }

        // ---- Katalog: Kategori ----
        public async Task<List<KategoriVm>> KategorileriGetirAsync()
        {
            var s = await _http.GetFromJsonAsync<ApiSonuc<List<KategoriVm>>>("/katalog/api/kategori");
            return s?.Data ?? new();
        }

        public async Task KategoriEkleAsync(string ad)
        {
            await BearerEkleAsync();
            await _http.PostAsJsonAsync("/katalog/api/kategori", new { Ad = ad });
        }

        public async Task KategoriSilAsync(string id)
        {
            await BearerEkleAsync();
            await _http.DeleteAsync($"/katalog/api/kategori/{id}");
        }

        // ---- Fotograf upload (multipart) ----
        public async Task<string?> FotografYukleAsync(IFormFile dosya)
        {
            await BearerEkleAsync();
            using var content = new MultipartFormDataContent();
            using var stream = dosya.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(dosya.ContentType);
            content.Add(fileContent, "dosya", dosya.FileName);

            var resp = await _http.PostAsync("/fotograf/api/fotograf", content);
            if (!resp.IsSuccessStatusCode) return null;

            var sonuc = await resp.Content.ReadFromJsonAsync<ApiSonuc<string>>();
            return sonuc?.Data;
        }

        // ---- Indirim ----
        public async Task<List<IndirimVm>> IndirimleriGetirAsync()
        {
            await BearerEkleAsync();
            var s = await _http.GetFromJsonAsync<ApiSonuc<List<IndirimVm>>>("/indirim/api/indirim");
            return s?.Data ?? new();
        }

        public async Task IndirimEkleAsync(IndirimVm dto)
        {
            await BearerEkleAsync();
            await _http.PostAsJsonAsync("/indirim/api/indirim", new
            {
                dto.UserId, dto.Oran, dto.Kod, dto.IsActive
            });
        }

        public async Task IndirimSilAsync(int id)
        {
            await BearerEkleAsync();
            await _http.DeleteAsync($"/indirim/api/indirim/{id}");
        }

        // ---- Siparis (admin: tum siparisler + durum guncelle) ----
        public async Task<List<SiparisVm>> TumSiparislerAsync()
        {
            await BearerEkleAsync();
            var s = await _http.GetFromJsonAsync<ApiSonuc<List<SiparisVm>>>("/siparis/api/siparis/tumu");
            return s?.Data ?? new();
        }

        // yeniDurum: 0..5 (SiparisDurum), kargoKodu opsiyonel
        public async Task SiparisDurumGuncelleAsync(int siparisId, int yeniDurum, string? kargoKodu = null)
        {
            await BearerEkleAsync();
            var url = $"/siparis/api/siparis/{siparisId}/durum/{yeniDurum}";
            if (!string.IsNullOrWhiteSpace(kargoKodu))
                url += $"?kargoKodu={Uri.EscapeDataString(kargoKodu)}";
            await _http.PutAsync(url, null);
        }
    }
}
