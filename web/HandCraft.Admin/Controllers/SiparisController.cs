using HandCraft.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Admin.Controllers
{
    public class SiparisController : Controller
    {
        private readonly AdminGatewayClient _gateway;

        public SiparisController(AdminGatewayClient gateway)
        {
            _gateway = gateway;
        }

        // Tum siparisler
        public async Task<IActionResult> Index()
        {
            var siparisler = await _gateway.TumSiparislerAsync();
            return View(siparisler);
        }

        // Genel durum guncelleme (0=Beklemede,1=Onaylandi,2=Hazirlaniyor,3=Kargoda,4=Teslim,5=Iptal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DurumGuncelle(int id, int durum, string? kargoKodu)
        {
            await _gateway.SiparisDurumGuncelleAsync(id, durum, kargoKodu);
            return RedirectToAction(nameof(Index));
        }
    }
}
