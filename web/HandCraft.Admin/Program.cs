using System.IdentityModel.Tokens.Jwt;
using HandCraft.Admin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HandCraft.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // ---- Gateway typed HttpClient ----
            builder.Services.AddHttpClient<AdminGatewayClient>(c =>
            {
                c.BaseAddress = new Uri(builder.Configuration["Gateway:BaseUrl"] ?? "http://localhost:7000");
            });

            // ---- Keycloak Admin API client (kullanici yonetimi) ----
            builder.Services.AddHttpClient<KeycloakAdminClient>();

            // ---- Keycloak OIDC (admin client) ----
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                // admin rolu olmayan kullanici icin: sonsuz redirect dongusunu onle.
                // AllowAnonymous olan Yetkisiz sayfasina yonlendir.
                options.AccessDeniedPath = "/Account/Yetkisiz";
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = builder.Configuration["Oidc:Authority"];
                options.ClientId = builder.Configuration["Oidc:ClientId"];
                options.ClientSecret = builder.Configuration["Oidc:ClientSecret"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.RequireHttpsMetadata = false;
                options.SaveTokens = true;
                // Claim'ler ID token'dan alinir; userinfo 'roles' dizisini dondurmedigi icin
                // userinfo'dan cekmeyi KAPAT (aksi halde admin rolu cookie'ye yazilmaz).
                options.GetClaimsFromUserInfoEndpoint = false;
                // KRITIK: Keycloak 'roles' claim'i schemas.microsoft.com/.../role URI'sine
                // maplenmesin; yoksa RoleClaimType="roles" bos duser ve admin1 "Yetkisiz" alir.
                options.MapInboundClaims = false;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = "roles";
                // HTTP (gelistirme) ortaminda correlation/nonce cookie'leri yazilabilsin:
                // SameSite=Lax + Secure=None. Aksi halde callback'te "Correlation failed" olur.
                options.NonceCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.NonceCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                // Keycloak 'roles' claim'i JSON dizisi olarak gelir; OIDC bunu tek claim'e
                // sikistirabilir ([\"admin\",\"customer\"]) ve IsInRole eslesmez. Burada
                // ID token JSON'undan 'roles' dizisini acip her rolu ayri 'roles' claim'i yapariz.
                options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                {
                    OnTokenValidated = ctx =>
                    {
                        var identity = ctx.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                        if (identity is not null)
                        {
                            // Mevcut 'roles' claim'lerini topla (tekil string veya JSON dizisi olabilir).
                            var ham = identity.FindAll("roles").Select(c => c.Value).ToList();
                            // Once eski 'roles' claim'lerini temizle, sonra duzgun haliyle ekle.
                            foreach (var c in identity.FindAll("roles").ToList())
                                identity.RemoveClaim(c);

                            foreach (var deger in ham)
                            {
                                var v = deger.Trim();
                                if (v.StartsWith("["))
                                {
                                    // JSON dizisi -> parcala
                                    try
                                    {
                                        foreach (var r in System.Text.Json.JsonSerializer.Deserialize<string[]>(v) ?? Array.Empty<string>())
                                            identity.AddClaim(new System.Security.Claims.Claim("roles", r));
                                    }
                                    catch { identity.AddClaim(new System.Security.Claims.Claim("roles", v)); }
                                }
                                else
                                {
                                    identity.AddClaim(new System.Security.Claims.Claim("roles", v));
                                }
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // ---- Admin rol zorunlu: tum site varsayilan olarak admin ister ----
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole("admin")
                    .Build();
            });

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
