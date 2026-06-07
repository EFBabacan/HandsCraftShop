using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandCraft.Admin.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login(string? returnUrl = "/")
        {
            return Challenge(
                new AuthenticationProperties { RedirectUri = returnUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        // admin rolu olmayan (or. customer) kullanici icin yetkisiz sayfasi
        public IActionResult Yetkisiz() => View();

        // GECICI TESHIS: cookie'deki claim'leri ve rol durumunu gosterir.
        public IActionResult WhoAmI()
        {
            return Json(new
            {
                isAuthenticated = User.Identity?.IsAuthenticated,
                name = User.Identity?.Name,
                isAdmin = User.IsInRole("admin"),
                roleClaimType = (User.Identity as System.Security.Claims.ClaimsIdentity)?.RoleClaimType,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}
