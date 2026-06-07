using HandCraft.Admin.Models;
using HandCraft.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Admin.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly KeycloakAdminClient _keycloak;

        public KullaniciController(KeycloakAdminClient keycloak)
        {
            _keycloak = keycloak;
        }

        public async Task<IActionResult> Index()
        {
            var kullanicilar = await _keycloak.KullanicilariGetirAsync();
            return View(kullanicilar);
        }

        public IActionResult Ekle() => View(new KullaniciFormVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(KullaniciFormVm form, string[] roller)
        {
            form.Roller = roller?.ToList() ?? new();
            if (form.Roller.Count == 0) form.Roller.Add("customer");

            var (ok, hata) = await _keycloak.KullaniciEkleAsync(form);
            if (!ok)
            {
                TempData["Hata"] = hata;
                return RedirectToAction(nameof(Ekle));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Duzenle(string id)
        {
            var k = await _keycloak.KullaniciGetirAsync(id);
            if (k is null) return NotFound();
            return View(new KullaniciFormVm
            {
                Id = k.Id, KullaniciAdi = k.KullaniciAdi, Email = k.Email,
                Ad = k.Ad, Soyad = k.Soyad, Aktif = k.Aktif, Roller = k.Roller
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(KullaniciFormVm form, string[] roller)
        {
            form.Roller = roller?.ToList() ?? new();
            await _keycloak.GuncelleAsync(form);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(string id)
        {
            await _keycloak.SilAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
