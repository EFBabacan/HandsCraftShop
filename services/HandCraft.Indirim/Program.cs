using HandCraft.Indirim.Services;
using HandCraft.Ortak.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HandCraft.Indirim
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---- Servisler ----
            builder.Services.AddScoped<IMyIndirimService, MyIndirimService>();
            builder.Services.AddScoped<IIdentityHelperService, IdentityHelperService>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            // ---- JWT (cift sema: varsayilan + ClientCredentialSchema) (Bolum 4 madde 4) ----
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.Authority = "http://localhost:8080/realms/handcraft";
                    x.Audience = "indirim";
                    x.RequireHttpsMetadata = false;
                    // Keycloak 'roles'/'preferred_username' otomatik URI'ye map'lenmesin (Bolum 9 tuzak).
                    x.MapInboundClaims = false;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        RoleClaimType = "roles",
                        NameClaimType = "preferred_username",
                        ClockSkew = TimeSpan.FromSeconds(15)
                    };
                })
                .AddJwtBearer("ClientCredentialSchema", x =>
                {
                    x.Authority = "http://localhost:8080/realms/handcraft";
                    x.Audience = "indirim";
                    x.RequireHttpsMetadata = false;
                    x.MapInboundClaims = false;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        RoleClaimType = "roles",
                        NameClaimType = "preferred_username"
                    };
                });

            builder.Services.AddAuthorization();

            // ---- OpenAPI + Swashbuckle (Bolum 4 madde 3) ----
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HandCraft Indirim API", Version = "v1" });
                var jwtScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Keycloak'tan alinan JWT'yi 'Bearer {token}' olarak girin.",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };
                c.AddSecurityDefinition("Bearer", jwtScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
            });

            var app = builder.Build();

            // ---- Acilista 'indirim' tablosunu hazirla (yoksa olustur) ----
            using (var scope = app.Services.CreateScope())
            {
                var indirimService = scope.ServiceProvider.GetRequiredService<IMyIndirimService>();
                indirimService.TabloyuHazirlaAsync().GetAwaiter().GetResult();
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
                // / -> /swagger yonlendirmesi (Bolum 4 madde 3)
                app.MapGet("/", ctx => { ctx.Response.Redirect("/swagger"); return Task.CompletedTask; });
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
