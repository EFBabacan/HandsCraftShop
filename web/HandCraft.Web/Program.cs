using System.IdentityModel.Tokens.Jwt;
using HandCraft.Ortak.Identity;
using HandCraft.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HandCraft.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // ---- Gateway typed HttpClient (tum backend cagrilari 7000 uzerinden) ----
            builder.Services.AddHttpClient<GatewayClient>(c =>
            {
                c.BaseAddress = new Uri(builder.Configuration["Gateway:BaseUrl"] ?? "http://localhost:7000");
            });

            // ---- Keycloak kayit client (musteri self-registration + profil) ----
            builder.Services.AddHttpClient<KeycloakRegisterClient>();

            // ---- Identity helper (token'dan userId/username) ----
            builder.Services.AddScoped<IIdentityHelperService, IdentityHelperService>();

            // ---- Keycloak OIDC (web client, code flow, cookie) (Bolum 7/Faz 8) ----
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = builder.Configuration["Oidc:Authority"];
                options.ClientId = builder.Configuration["Oidc:ClientId"];
                options.ClientSecret = builder.Configuration["Oidc:ClientSecret"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.RequireHttpsMetadata = false;
                options.SaveTokens = true;          // access_token cookie'de saklanir
                options.GetClaimsFromUserInfoEndpoint = true;
                // Realm'de standart 'profile' scope'u yok; claim'ler username-flat/roles-flat/sub-flat ile geliyor.
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = "roles";
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
