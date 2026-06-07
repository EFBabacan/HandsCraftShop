using HandCraft.Admin.Models;
using HandCraft.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Admin.Controllers
{
    public class IndirimController : Controller
    {
        private readonly AdminGatewayClient _gateway;

        public IndirimController(AdminGatewayClient gateway)
        {
            _gateway = gateway;
        }

        public async Task<IActionResult> Index()
        {
            var indirimler = await _gateway.IndirimleriGetirAsync();
            return View(indirimler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(IndirimVm form)
        {
            if (string.IsNullOrWhiteSpace(form.UserId)) form.UserId = "GENEL";
            await _gateway.IndirimEkleAsync(form);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            await _gateway.IndirimSilAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
