using HandCraft.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Admin.Controllers
{
    public class KategoriController : Controller
    {
        private readonly AdminGatewayClient _gateway;

        public KategoriController(AdminGatewayClient gateway)
        {
            _gateway = gateway;
        }

        public async Task<IActionResult> Index()
        {
            var kategoriler = await _gateway.KategorileriGetirAsync();
            return View(kategoriler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(string ad)
        {
            if (!string.IsNullOrWhiteSpace(ad))
                await _gateway.KategoriEkleAsync(ad);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(string id)
        {
            await _gateway.KategoriSilAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
