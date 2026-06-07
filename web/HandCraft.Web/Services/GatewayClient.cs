using System.Net.Http.Headers;
using System.Net.Http.Json;
using HandCraft.Web.Models;
using Microsoft.AspNetCore.Authentication;

namespace HandCraft.Web.Services
{
    // Tum backend cagrilarini SADECE Gateway (7000) uzerinden yapar.
    // Giris yapilmissa access token'i Bearer olarak iletir (Bolum 7/Faz 8).
    public class GatewayClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GatewayClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task BearerEkleAsync()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx?.User?.Identity?.IsAuthenticated == true)
            {
                var token = await ctx.GetTokenAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ---- Katalog (public) ----
        public async Task<List<UrunVm>> UrunleriGetirAsync()
        {
            var sonuc = await _http.GetFromJsonAsync<ApiSonuc<List<UrunVm>>>("/katalog/api/urun");
            return sonuc?.Data ?? new();
        }

        public async Task<UrunVm?> UrunGetirAsync(string id)
        {
            var sonuc = await _http.GetFromJsonAsync<ApiSonuc<UrunVm>>($"/katalog/api/urun/{id}");
            return sonuc?.Data;
        }

        public async Task<List<KategoriVm>> KategorileriGetirAsync()
        {
            var sonuc = await _http.GetFromJsonAsync<ApiSonuc<List<KategoriVm>>>("/katalog/api/kategori");
            return sonuc?.Data ?? new();
        }

        public async Task<List<UrunVm>> KategoriUrunleriAsync(string kategoriId)
        {
            var sonuc = await _http.GetFromJsonAsync<ApiSonuc<List<UrunVm>>>($"/katalog/api/urun/kategori/{kategoriId}");
            return sonuc?.Data ?? new();
        }

        // ---- Sepet (Bearer) ----
        public async Task<SepetVm> SepetGetirAsync()
        {
            await BearerEkleAsync();
            var sonuc = await _http.GetFromJsonAsync<ApiSonuc<SepetVm>>("/sepet/api/sepet");
            return sonuc?.Data ?? new();
        }

        public async Task SepeteEkleAsync(SepeteEkleVm item)
        {
            await BearerEkleAsync();
            await _http.PostAsJsonAsync("/sepet/api/sepet", item);
        }

        public async Task SepettenSilAsync(string urunId)
        {
            await BearerEkleAsync();
            await _http.DeleteAsync($"/sepet/api/sepet/{urunId}");
        }

        public async Task SepetiTemizleAsync()
        {
            await BearerEkleAsync();
            await _http.DeleteAsync("/sepet/api/sepet");
        }

        // ---- Siparis (Bearer) ----
        public async Task<List<SiparisVm>> SiparislerimAsync()
        {
            await BearerEkleAsync();
            var sonuc = await _http.GetFromJsonAsync<ApiSonuc<List<SiparisVm>>>("/siparis/api/siparis");
            return sonuc?.Data ?? new();
        }

        // ---- Odeme (Bearer) ----
        public async Task<bool> OdemeYapAsync(OdemeAlVm odeme)
        {
            await BearerEkleAsync();
            var resp = await _http.PostAsJsonAsync("/odeme/api/odeme", odeme);
            return resp.IsSuccessStatusCode;
        }

        // ---- Yorum ----
        public async Task<YorumOzetVm> UrunYorumlariAsync(string urunId)
        {
            var sonuc = await _http.GetFromJsonAsync<ApiSonuc<YorumOzetVm>>($"/katalog/api/yorum/urun/{urunId}");
            return sonuc?.Data ?? new();
        }

        public async Task<bool> YorumEkleAsync(string urunId, int puan, string metin)
        {
            await BearerEkleAsync();
            var resp = await _http.PostAsJsonAsync("/katalog/api/yorum", new { UrunId = urunId, Puan = puan, Metin = metin });
            return resp.IsSuccessStatusCode;
        }

        // ---- Indirim (Bearer) ----
        // Kod ile aktif indirimi getirir; bulunamazsa null.
        public async Task<IndirimVm?> IndirimKoduGetirAsync(string kod)
        {
            await BearerEkleAsync();
            var resp = await _http.GetAsync($"/indirim/api/indirim/kod/{Uri.EscapeDataString(kod)}");
            if (!resp.IsSuccessStatusCode) return null;
            var sonuc = await resp.Content.ReadFromJsonAsync<ApiSonuc<IndirimVm>>();
            return sonuc?.Data;
        }
    }
}
