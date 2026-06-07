using HandCraft.Web.Models;
using HandCraft.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Web.Controllers
{
    [Authorize]
    public class SiparisController : Controller
    {
        private readonly GatewayClient _gateway;

        public SiparisController(GatewayClient gateway)
        {
            _gateway = gateway;
        }

        // Checkout sayfasi: sepeti goster, adres formu, opsiyonel indirim kodu.
        public async Task<IActionResult> Checkout(string? indirimKodu)
        {
            var sepet = await _gateway.SepetGetirAsync();
            if (sepet.Urunler.Count == 0)
                return RedirectToAction("Index", "Sepet");

            var vm = new CheckoutVm { Sepet = sepet };

            // Indirim kodu girildiyse Indirim API'den dogrula.
            if (!string.IsNullOrWhiteSpace(indirimKodu))
            {
                var indirim = await _gateway.IndirimKoduGetirAsync(indirimKodu.Trim());
                if (indirim is not null && indirim.IsActive)
                {
                    vm.IndirimKodu = indirim.Kod;
                    vm.IndirimOrani = indirim.Oran;
                    vm.IndirimMesaji = $"'{indirim.Kod}' kodu uygulandi: %{indirim.Oran} indirim.";
                }
                else
                {
                    vm.IndirimMesaji = "Gecersiz veya pasif indirim kodu.";
                }
            }

            return View(vm);
        }

        // Odeme: sahte odeme -> Odeme API -> RabbitMQ -> Siparis (MSSQL). Sonra sepeti temizle.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Odeme(string il, string ilce, string acikAdres, string kartSahibi, string kartNo, string? indirimKodu)
        {
            var sepet = await _gateway.SepetGetirAsync();
            if (sepet.Urunler.Count == 0)
                return RedirectToAction("Index", "Sepet");

            // Indirim kodunu odeme aninda yeniden dogrula (guvenlik: client'a guvenme).
            decimal oran = 0m;
            if (!string.IsNullOrWhiteSpace(indirimKodu))
            {
                var indirim = await _gateway.IndirimKoduGetirAsync(indirimKodu.Trim());
                if (indirim is not null && indirim.IsActive)
                    oran = indirim.Oran;
            }

            // Indirimi her urun fiyatina yansit (hem Odeme hem Siparis indirimli tutari kaydeder).
            var carpan = (100m - oran) / 100m;
            var odeme = new OdemeAlVm
            {
                Il = il,
                Ilce = ilce,
                AcikAdres = acikAdres,
                KartSahibi = kartSahibi,
                KartNo = kartNo,
                Urunler = sepet.Urunler.Select(u => new OdemeUrunVm
                {
                    UrunId = u.UrunId,
                    UrunAdi = u.UrunAdi,
                    Fiyat = Math.Round(u.Fiyat * carpan, 2),
                    Adet = u.Adet
                }).ToList()
            };

            var basarili = await _gateway.OdemeYapAsync(odeme);
            if (!basarili)
            {
                TempData["Hata"] = "Odeme alinamadi, lutfen tekrar deneyin.";
                return RedirectToAction(nameof(Checkout));
            }

            // Odeme basarili -> sepeti temizle, siparis asenkron olusur.
            await _gateway.SepetiTemizleAsync();
            return RedirectToAction(nameof(Basarili));
        }

        public IActionResult Basarili() => View();

        // Siparislerim
        public async Task<IActionResult> Index()
        {
            var siparisler = await _gateway.SiparislerimAsync();
            return View(siparisler);
        }
    }
}
