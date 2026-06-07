using HandCraft.Web.Models;
using HandCraft.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Web.Controllers
{
    [Authorize]
    public class SepetController : Controller
    {
        private readonly GatewayClient _gateway;

        public SepetController(GatewayClient gateway)
        {
            _gateway = gateway;
        }

        public async Task<IActionResult> Index()
        {
            var sepet = await _gateway.SepetGetirAsync();
            return View(sepet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(string urunId)
        {
            var urun = await _gateway.UrunGetirAsync(urunId);
            if (urun is not null)
            {
                await _gateway.SepeteEkleAsync(new SepeteEkleVm
                {
                    UrunId = urun.Id,
                    UrunAdi = urun.Ad,
                    Fiyat = urun.Fiyat,
                    Adet = 1,
                    ImageUrl = urun.ImageUrl
                });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(string urunId)
        {
            await _gateway.SepettenSilAsync(urunId);
            return RedirectToAction(nameof(Index));
        }
    }
}
