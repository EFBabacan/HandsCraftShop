using HandCraft.Admin.Models;
using HandCraft.Admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HandCraft.Admin.Controllers
{
    public class UrunController : Controller
    {
        private readonly AdminGatewayClient _gateway;

        public UrunController(AdminGatewayClient gateway)
        {
            _gateway = gateway;
        }

        public async Task<IActionResult> Index()
        {
            var urunler = await _gateway.UrunleriGetirAsync();
            return View(urunler);
        }

        public async Task<IActionResult> Ekle()
        {
            await KategorileriYukleAsync();
            return View(new UrunFormVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(UrunFormVm form, IFormFile? gorsel)
        {
            if (gorsel is not null && gorsel.Length > 0)
            {
                var url = await _gateway.FotografYukleAsync(gorsel);
                if (url is not null) form.ImageUrl = url;
            }
            await _gateway.UrunEkleAsync(form);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Duzenle(string id)
        {
            var urun = await _gateway.UrunGetirAsync(id);
            if (urun is null) return NotFound();

            await KategorileriYukleAsync();
            return View(new UrunFormVm
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Aciklama = urun.Aciklama,
                Fiyat = urun.Fiyat,
                KategoriId = urun.KategoriId,
                ImageUrl = urun.ImageUrl,
                Stok = urun.Stok
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(UrunFormVm form, IFormFile? gorsel)
        {
            if (gorsel is not null && gorsel.Length > 0)
            {
                var url = await _gateway.FotografYukleAsync(gorsel);
                if (url is not null) form.ImageUrl = url;
            }
            await _gateway.UrunGuncelleAsync(form);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(string id)
        {
            await _gateway.UrunSilAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task KategorileriYukleAsync()
        {
            var kategoriler = await _gateway.KategorileriGetirAsync();
            ViewBag.Kategoriler = kategoriler
                .Select(k => new SelectListItem { Value = k.Id, Text = k.Ad })
                .ToList();
        }
    }
}
