using HandCraft.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly KeycloakRegisterClient _register;

        public AccountController(KeycloakRegisterClient register)
        {
            _register = register;
        }

        // Keycloak'a OIDC challenge -> login
        public IActionResult Login(string? returnUrl = "/")
        {
            return Challenge(
                new AuthenticationProperties { RedirectUri = returnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        // Hem cookie hem Keycloak oturumunu kapat
        public IActionResult Logout()
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        // Musteri kayit formu
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string kullaniciAdi, string email, string ad, string soyad, string parola)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(parola))
            {
                ViewBag.Hata = "Kullanici adi ve parola zorunlu.";
                return View();
            }

            var (ok, hata) = await _register.MusteriKaydetAsync(kullaniciAdi, email, ad, soyad, parola);
            if (!ok)
            {
                ViewBag.Hata = hata;
                return View();
            }

            // Kayit basarili. E-posta verildiyse dogrulama maili gonderildi.
            ViewBag.Basarili = string.IsNullOrWhiteSpace(email)
                ? "Kaydiniz olusturuldu. Simdi giris yapabilirsiniz."
                : "Kaydiniz olusturuldu. E-postaniza gonderilen dogrulama linkine tiklayin, sonra giris yapin.";
            return View();
        }
    }
}
