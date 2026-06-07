using System.Diagnostics;
using HandCraft.Web.Models;
using HandCraft.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly GatewayClient _gateway;

        public HomeController(GatewayClient gateway)
        {
            _gateway = gateway;
        }

        // Vitrin: tum urunler + kategoriler. Kategori secilirse filtreler.
        public async Task<IActionResult> Index(string? kategoriId)
        {
            var urunler = string.IsNullOrEmpty(kategoriId)
                ? await _gateway.UrunleriGetirAsync()
                : await _gateway.KategoriUrunleriAsync(kategoriId);

            ViewBag.Kategoriler = await _gateway.KategorileriGetirAsync();
            ViewBag.SeciliKategori = kategoriId;
            return View(urunler);
        }

        public async Task<IActionResult> Detay(string id)
        {
            var urun = await _gateway.UrunGetirAsync(id);
            if (urun is null) return NotFound();
            var yorumlar = await _gateway.UrunYorumlariAsync(id);
            return View(new UrunDetayVm { Urun = urun, Yorumlar = yorumlar });
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumEkle(string urunId, int puan, string metin)
        {
            await _gateway.YorumEkleAsync(urunId, puan, metin);
            return RedirectToAction(nameof(Detay), new { id = urunId });
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
