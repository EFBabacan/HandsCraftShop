using HandCraft.Ortak.Identity;
using HandCraft.Web.Models;
using HandCraft.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Web.Controllers
{
    [Authorize]
    public class ProfilController : Controller
    {
        private readonly KeycloakRegisterClient _keycloak;
        private readonly IIdentityHelperService _identity;

        public ProfilController(KeycloakRegisterClient keycloak, IIdentityHelperService identity)
        {
            _keycloak = keycloak;
            _identity = identity;
        }

        // Hesabim sayfasi
        public async Task<IActionResult> Index()
        {
            var userId = _identity.GetUserId();
            var (ad, soyad, email) = await _keycloak.ProfilGetirAsync(userId);
            return View(new ProfilVm
            {
                KullaniciAdi = _identity.GetUserName(),
                Ad = ad, Soyad = soyad, Email = email
            });
        }

        // Profil bilgilerini guncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(string ad, string soyad, string email)
        {
            var userId = _identity.GetUserId();
            await _keycloak.ProfilGuncelleAsync(userId, ad, soyad, email);
            TempData["Mesaj"] = "Bilgileriniz guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // Sifre degistir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreDegistir(string yeniParola)
        {
            if (string.IsNullOrWhiteSpace(yeniParola) || yeniParola.Length < 4)
            {
                TempData["Hata"] = "Parola en az 4 karakter olmali.";
                return RedirectToAction(nameof(Index));
            }
            var userId = _identity.GetUserId();
            await _keycloak.SifreDegistirAsync(userId, yeniParola);
            TempData["Mesaj"] = "Parolaniz degistirildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
