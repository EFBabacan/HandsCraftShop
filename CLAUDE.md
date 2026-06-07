# HandCraft Shop — Mikroservis E-Ticaret · Claude Code Master Planı

> **Amaç:** Mikroservis Mimarisi dersi için, hocanın referans reposu (`serdarpacaci/ISUBUBLG-423_2026_2` — *IsubuSatis*) ile **birebir aynı stilde** ama 12 isterin tamamını karşılayan, çalışan bir el-işi ürün satış platformu (`HandCraft Shop`) üretmek.
> **Kullanım:** Bu doküman Claude Code'a hem bağlam (`CLAUDE.md`) hem de fazlı yol haritasıdır. Fazları **sırayla** çalıştır. Her fazın sonunda "Kabul kriteri" karşılanmadan bir sonrakine geçme.

---

## 0. Karar Özeti (kilitlendi)

| Konu | Karar | Gerekçe |
|------|-------|---------|
| Kimlik sunucusu (#7) | **Keycloak 26.4** | Hocanın reposu Keycloak kullanıyor; IdentityServer4 zaten 2022'de EOL. Hocaya "ders reposuyla aynı identity server" diye sunulur. |
| Public site + Admin (#2,#3) | **ASP.NET Core MVC (Razor)** | .NET dünyasıyla en uyumlu, en az sürtünme. |
| API Gateway (#12) | **YARP (Yarp.ReverseProxy)** | Microsoft'un .NET 9 yerel reverse-proxy'si. (Alternatif: Ocelot — istersen değiştirilebilir.) |
| Domain | El işi ürünler (handmade) — **HandCraft Shop** | — |
| .NET sürümü | **net9.0** (repoyla aynı) | — |
| Dil/isimlendirme | Türkçe domain isimleri (`Katalog`, `Sepet`, `Siparis`, `Indirim`, `Odeme`, `Fotograf`, `Ortak`) + `HandCraft.*` namespace | Hocanın stilini korur, notlandırmada tanıdık gelir. |

---

## 1. İster → Servis Eşlemesi (Kabul Tablosu)

Her isterin hangi projede karşılandığı ve hocaya **nasıl ispatlanacağı**:

| # | İster | Karşılayan parça | Demo / İspat |
|---|-------|------------------|--------------|
| 1 | .NET 8/9/10 web uygulaması çalışıyor mu | Tüm projeler `net9.0` | `dotnet run` → Swagger / site açılıyor |
| 2 | Public site | `HandCraft.Web` (MVC) | Ürün listele → sepete ekle → sipariş ver |
| 3 | Admin yönetimi | `HandCraft.Admin` (MVC) | Ürün/kategori/indirim CRUD, sipariş görüntüleme |
| 4 | MS SQL Server | `HandCraft.Siparis.Persistence` (EF Core + SqlServer) | `Siparis` tablosu SSMS/Azure Data Studio'da |
| 5 | NoSQL | `HandCraft.Katalog` (MongoDB) | mongo-express'te `urun`/`kategori` koleksiyonları |
| 6 | MSSQL harici ilişkisel | `HandCraft.Indirim` (PostgreSQL + Dapper) | pgAdmin'de `indirim` tablosu |
| 7 | IdentityServer4 | **Keycloak** (`realms/handcraft`) | Keycloak admin paneli, login akışı, JWT |
| 8 | Redis | `HandCraft.Sepet` (StackExchange.Redis) | redis-commander'da sepet anahtarları |
| 9 | CQRS | `HandCraft.Siparis.Application` (MediatR) | `Commands`/`Queries` klasörleri + handler'lar |
| 10 | RabbitMQ | `Odeme` publish → `Siparis` consume (MassTransit) | RabbitMQ UI'da `siparis-olustur-service` kuyruğu |
| 11 | Clean/Onion | `Siparis` = Domain→Persistence→Application→Api | Katman bağımlılık yönü, proje referansları |
| 12 | API Gateway | `HandCraft.Gateway` (YARP) | Tüm istekler gateway üzerinden route'lanıyor |

---

## 2. Hedef Solution Yapısı

```
HandCraftShop/
├── HandCraftShop.sln
│
├── shared/
│   └── HandCraft.Ortak/                  # Paylaşılan: ServisSonuc<T>, hata DTO, validation filter,
│                                         # IIdentityHelperService, RabbitMQ mesaj kontratları
│
├── services/
│   ├── HandCraft.Katalog/                # [#5] MongoDB — Urun + Kategori (REST API)
│   ├── HandCraft.Fotograf/               # Görsel yükleme API'si (URL döner)
│   ├── HandCraft.Indirim/                # [#6] PostgreSQL + Dapper — indirim kodları
│   ├── HandCraft.Sepet/                  # [#8] Redis — kullanıcı sepeti
│   ├── HandCraft.Odeme/                  # [#10] RabbitMQ publisher (MassTransit)
│   │
│   └── HandCraft.Siparis/                # [#4,#9,#11] Clean Architecture
│       ├── HandCraft.Siparis.Domain/         # Entity'ler (bağımsız katman)
│       ├── HandCraft.Siparis.Persistence/    # EF Core + SqlServer + Migrations
│       ├── HandCraft.Siparis.Application/    # MediatR (CQRS) + MassTransit consumer
│       └── HandCraft.Siparis.Api/            # Controller + Program.cs wiring
│
├── gateway/
│   └── HandCraft.Gateway/                # [#12] YARP reverse proxy
│
├── web/
│   ├── HandCraft.Web/                    # [#2] Public MVC site (müşteri)
│   └── HandCraft.Admin/                  # [#3] Admin MVC paneli
│
└── infra/
    ├── keycloak/        docker-compose.yml + realm-export.json   # [#7]
    ├── mongo/           docker-compose.yml                       # [#5]
    ├── redis/           docker-compose.yml                       # [#8]
    ├── postgres/        docker-compose.yml                       # [#6] (Indirim için, Keycloak'tan ayrı)
    ├── rabbitmq/        docker-compose.yml                       # [#10]
    ├── mssql/           docker-compose.yml                       # [#4]
    └── docker-compose.all.yml                                    # Hepsini tek komutla ayağa kaldırma
```

> Not: Hocanın reposunda projeler kökte düz duruyor. Bizimki klasörlü olabilir; istersen Claude Code'a "projeleri kökte düz tut" dedirtebilirsin. Notlandırmayı etkilemez.

---

## 3. Port Planı (çakışma olmasın)

| Bileşen | Host Port |
|---------|-----------|
| Keycloak | 8080 |
| Keycloak Postgres | 5432 |
| Indirim PostgreSQL | 5433 |
| pgAdmin | 8888 |
| MongoDB | 27017 · mongo-express 8081 |
| Redis | 6379 · redis-commander 8082 |
| RabbitMQ | 5672 · management UI 15672 |
| MS SQL Server | 1433 |
| **Gateway** (tek giriş) | 7000 |
| Katalog API | 7001 |
| Sepet API | 7002 |
| Siparis API | 7003 |
| Indirim API | 7004 |
| Odeme API | 7005 |
| Fotograf API | 7006 |
| **Web (public)** | 5000 |
| **Admin** | 5001 |

---

## 4. Hocanın "Ev Stili" — Mutlaka Uyulacak Konvansiyonlar

Bunlar referans repodan çıkarıldı. Claude Code bu kurallara **harfiyen** uymalı:

1. **Namespace:** `HandCraft.<Servis>` (örn. `HandCraft.Katalog`, `HandCraft.Siparis.Application`).
2. **Program.cs:** top-level statement DEĞİL — klasik `public class Program { public static void Main(string[] args) {...} }` kalıbı.
3. **Swagger:** `AddOpenApi()` + Swashbuckle birlikte; development'ta `/` → `/swagger` redirect:
   ```csharp
   app.MapGet("/", ctx => { ctx.Response.Redirect("/swagger"); return Task.CompletedTask; });
   ```
4. **JWT (her API'de):** çift şema — varsayılan + `"ClientCredentialSchema"`:
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(x => {
         x.Authority = "http://localhost:8080/realms/handcraft";
         x.Audience = "<servis-adı>";          // katalog, sepet, siparis, indirim, odeme, fotograf
         x.RequireHttpsMetadata = false;
         x.TokenValidationParameters = new() {
             RequireExpirationTime = true, ValidateAudience = true,
             ValidateIssuerSigningKey = true, ValidateLifetime = true, ValidateIssuer = true,
             RoleClaimType = "roles", NameClaimType = "preferred_username",
             ClockSkew = TimeSpan.FromSeconds(15)
         };
     })
     .AddJwtBearer("ClientCredentialSchema", x => { /* aynı authority/audience, sade validation */ });
   builder.Services.AddHttpContextAccessor();
   ```
   Pipeline: `UseAuthentication()` → `UseAuthorization()`.
5. **Sonuç sarmalayıcı:** `HandCraft.Ortak.ServisSonuc` / `ServisSonuc<T>` (statik `Basarili()` / `Hata()` fabrikaları). Controller'lar bunu döndürür.
6. **Validation:** FluentValidation + `IsubuValidationFilter<T>` benzeri `HandCraftValidationFilter<T>` (IEndpointFilter).
7. **Identity helper:** `IIdentityHelperService` (`GetUserId()`, `GetUserName()`) — HttpContext claim'lerinden okur. UserId = `ClaimTypes.NameIdentifier`.
8. **Mongo:** `IOptions<MongoDbSettings>` → `MongoClient` → `IMongoCollection<T>`; tablo isimleri `MongoDbTables` sabitlerinde; AutoMapper ile DTO map.
9. **Redis:** `RedisService` (singleton), `ConnectionMultiplexer.Connect($"{host}:{port}")`, `GetDatabase(db)`.
10. **PostgreSQL/Indirim:** Dapper + `NpgsqlConnection`, ham SQL (`select * from indirim` vb.) — EF Core DEĞİL.
11. **RabbitMQ:** MassTransit; publisher `ISendEndpointProvider.GetSendEndpoint(new Uri("queue:siparis-olustur-service"))`; mesaj kontratı `HandCraft.Ortak`'ta (`SiparisOlusturMessageCommand`).
12. **CQRS:** MediatR; `Commands/`, `Queries/` klasörleri, altlarında `Dtos/` ve `Handlers/`; `IRequest<TResponse>` + `IRequestHandler<,>`.
13. **EF Core/Siparis:** `DbContext` Persistence katmanında; migration'lar Persistence assembly'sinde; Api'de `MigrationsAssembly("HandCraft.Siparis.Persistence")`.
14. **Docker:** her altyapı kendi `docker-compose.yml`'inde (UI container'ları dahil: redis-commander, mongo-express, pgAdmin, RabbitMQ management).

> Referans repo (stil için): `https://github.com/serdarpacaci/ISUBUBLG-423_2026_2`

---

## 5. Keycloak Realm Konfigürasyonu (#7)

**Realm:** `handcraft`

**Roller (realm roles):** `customer`, `admin`

**Client'lar:**

| Client ID | Tip | Kullanım |
|-----------|-----|----------|
| `web` | confidential, Standard Flow (OIDC code) | Public MVC login |
| `admin` | confidential, Standard Flow | Admin MVC login |
| `katalog`,`sepet`,`siparis`,`indirim`,`odeme`,`fotograf` | bearer-only / audience | API doğrulama hedefleri |

**Önemli ayarlar:**
- Her API'nin `Audience`'ı kendi client/scope adına denk gelmeli (`aud` claim). Client scope ile `audience mapper` ekle.
- `roles` claim'i token'a binsin (realm role mapper).
- Test kullanıcıları: `musteri1` (rol: customer), `admin1` (rol: admin).
- Realm'i `realm-export.json` olarak dışa aktar ki Claude Code import edebilsin / yeniden kurulabilir olsun.

---

## 6. Uçtan Uca Akış (referans + tamamlanmış hali)

```
[Web/MVC] Keycloak ile login (OIDC) → access token
   │
   ├─ Ürünleri listele  → Gateway → Katalog(Mongo)        [public read]
   ├─ Sepete ekle       → Gateway → Sepet(Redis)          [Bearer]
   ├─ İndirim uygula    → Gateway → Indirim(Postgres)     [Bearer]
   └─ Ödeme yap         → Gateway → Odeme
                                      │  ödeme başarılı
                                      └─ MassTransit.Send → queue:siparis-olustur-service
                                                                   │
                                                          [Siparis.Application Consumer]
                                                                   │ MediatR
                                                          CreateSiparisCommandHandler
                                                                   │ EF Core
                                                          MS SQL Server'a sipariş kaydı
                                                                   └─ Sepeti temizle
```

> Referans repoda `CreateSiparisCommandHandler` ve Siparis consumer **stub**. Bu plan onları **gerçekten çalışır** hale getirir (DB'ye yazma + sepet temizleme).

---

## 7. Fazlı Yol Haritası

> Her faz bağımsız çalıştırılabilir bir Claude Code görevidir. Sıra önemlidir: altyapı → veri servisleri → sipariş/mesajlaşma → gateway → frontend → entegrasyon.

### Faz 0 — Solution iskeleti + Ortak + Altyapı
- `HandCraftShop.sln` + tüm proje iskeletleri (boş ama derlenen).
- `HandCraft.Ortak`: `ServisSonuc<T>`, `ServisHataDto`, `HandCraftValidationFilter<T>`, `IIdentityHelperService`/`IdentityHelperService`, `SiparisOlusturMessageCommand` + `SiparisDto`.
- Tüm `docker-compose` dosyaları + `docker-compose.all.yml`.
- Keycloak `handcraft` realm'i, client'lar, roller, test kullanıcıları, `realm-export.json`.
- **Kabul:** `dotnet build` temiz; `docker compose -f infra/docker-compose.all.yml up -d` ile tüm altyapı ayakta; Keycloak admin paneli (8080) açılıyor, `handcraft` realm'i mevcut.

### Faz 1 — Katalog (MongoDB) [#5]
- `Urun` (el-işi ürün: Ad, Aciklama, Fiyat, KategoriId, ImageUrl, Stok, EklenmeTarihi), `Kategori` (Ad).
- `IUrunService`/`UrunService`, `IKategoriService`/`KategoriService`, AutoMapper, DTO'lar.
- `UrunController`, `KategoriController` (okuma public, yazma `admin` rol).
- 10-12 el-işi ürün + 3-4 kategori (Seramik, Ahşap, Takı, Örgü) seed.
- **Kabul:** Swagger'dan ürün listele/ekle çalışıyor; mongo-express'te koleksiyonlar görünüyor.

### Faz 2 — Fotograf API
- `POST /api/fotograf` (multipart) → dosyayı `wwwroot/images`'a kaydet, URL döndür. JWT `admin`.
- Katalog ürünleri bu URL'yi `ImageUrl`'de tutar.
- **Kabul:** Görsel yükleniyor, dönen URL tarayıcıda açılıyor.

### Faz 3 — Indirim (PostgreSQL + Dapper) [#6]
- `IndirimDto` (Id, UserId, Oran, Kod, IsActive), `MyIndirimService` (Dapper, `NpgsqlConnection`, ham SQL).
- `IndirimController` CRUD; uygulama açılışında `indirim` tablosunu yoksa oluşturan SQL.
- **Kabul:** pgAdmin'de `indirim` tablosu + kayıtlar; "Kod ile indirim getir" endpoint'i çalışıyor.

### Faz 4 — Sepet (Redis) [#8]
- `SepetDto` (UserId, `List<SepetItemDto>`), item (UrunId, UrunAdi, Fiyat, Adet, ImageUrl).
- `RedisService` (singleton) + `ISepetService`/`MySepetService` (key: `sepet:{userId}`).
- `SepetController`: getir / ekle / sil / temizle. JWT `customer`. UserId token'dan (`IIdentityHelperService`).
- **Kabul:** redis-commander'da `sepet:{userId}` anahtarı; ekle/sil çalışıyor.

### Faz 5 — Siparis (Clean Architecture + CQRS + MSSQL) [#4,#9,#11]
- **Domain:** `BaseEntity<T>`, `CreationalEntity<T>`, `Siparis`, `Address`, `SiparisUrunBilgi`.
- **Persistence:** `SiparisDbContext` (DbSet'ler), EF migration (`Initial`), `MigrationsAssembly` ayarı.
- **Application:** MediatR — `CreateSiparisCommand` + handler (**DB'ye gerçek kayıt**), `GetSiparislerByUserIdQuery` + handler, DTO'lar.
- **Api:** `SiparisController` (GET kendi siparişlerim, POST oluştur), Program.cs (EF + MediatR + JWT).
- **Kabul:** `dotnet ef database update` çalışıyor; POST ile sipariş MSSQL'e yazılıyor; GET ile geri okunuyor.

### Faz 6 — Odeme + RabbitMQ Mesajlaşma [#10]
- **Odeme:** `OdemeController.OdemeAl` → sahte ödeme → `SiparisOlusturMessageCommand`'i `queue:siparis-olustur-service`'e gönder (MassTransit + RabbitMQ).
- **Siparis.Application:** `SiparisOlusturConsumer` (MassTransit) → mesajı MediatR `CreateSiparisCommand`'e çevir → sipariş kaydet → (opsiyonel) sepeti temizle.
- Siparis.Api ve Odeme Program.cs'lerinde MassTransit + RabbitMQ host wiring.
- **Kabul:** Odeme'ye istek → RabbitMQ UI'da kuyruk hareketi → Siparis tablosuna yeni kayıt (uçtan uca).

### Faz 7 — API Gateway (YARP) [#12]
- `HandCraft.Gateway`: YARP `ReverseProxy` config (appsettings) ile route'lar:
  - `/katalog/**` → 7001, `/sepet/**` → 7002, `/siparis/**` → 7003, `/indirim/**` → 7004, `/odeme/**` → 7005, `/fotograf/**` → 7006.
- Bearer token pass-through; CORS Web/Admin için açık.
- **Kabul:** Tüm API çağrıları `http://localhost:7000/...` üzerinden çalışıyor; doğrudan servis portu MVC'lerde kullanılmıyor.

### Faz 8 — Public Site (HandCraft.Web, MVC) [#2]
- Keycloak OIDC login (`web` client, code flow, cookie).
- Sayfalar: Ana sayfa (ürün vitrini), Kategori/ürün listesi, Ürün detay, Sepet, Ödeme/Checkout, Siparişlerim, Login/Logout.
- Backend çağrıları **yalnızca Gateway** üzerinden; access token Bearer olarak iletilir.
- Sade ama düzgün Razor + Bootstrap UI; el-işi temasına uygun.
- **Kabul:** Giriş → ürün gez → sepete ekle → checkout → sipariş oluştu → "Siparişlerim"de görünüyor.

### Faz 9 — Admin Paneli (HandCraft.Admin, MVC) [#3]
- Keycloak OIDC login + `admin` rol kontrolü (yetkisiz erişim engelli).
- Yönetim: Ürün CRUD (+ Fotograf yükleme), Kategori CRUD, İndirim CRUD, Sipariş listesi/detay.
- Gateway üzerinden çağrı.
- **Kabul:** admin1 ile giriş → ürün ekle/sil → public sitede anında görünür; customer rolü admin'e giremiyor.

### Faz 10 — Entegrasyon, Çalıştırma & Doğrulama
- `RUN.md`: sırayla ayağa kaldırma adımları + tek komutluk script(ler).
- 12 isteri tek tek ispatlayan **demo senaryosu** (hoca sunumu için).
- Smoke test: tüm servisler /health veya /swagger veriyor.
- **Kabul:** Sıfırdan `docker compose up` + servis `dotnet run`'ları ile uçtan uca senaryo çalışıyor; Bölüm 1'deki tablo bütünüyle ✅.

---

## 8. Claude Code'a Verilecek Promptlar

> Önce aşağıdaki **Master Prompt**'u ver (bağlam oturur). Sonra her fazı kendi promptuyla sırayla çalıştır. Claude Code büyük işleri tek seferde değil, fazlara bölünce çok daha sağlam üretir.

### 8.0 — Master Prompt (en başta bir kez)

```
HandCraft Shop adında bir mikroservis e-ticaret projesi geliştireceğiz (el-işi/handmade
ürün satışı). Hedef: bir üniversite "Mikroservis Mimarisi" dersinin 12 isterini karşılamak
ve şu referans reponun stiliyle BİREBİR uyumlu olmak:
https://github.com/serdarpacaci/ISUBUBLG-423_2026_2

Bu dosyayı (HandCraftShop_ClaudeCode_Plan.md) projenin kök dizinine koy ve CLAUDE.md olarak
referans al. Tüm kararlar, klasör yapısı, port planı, "Ev Stili Konvansiyonları" (Bölüm 4),
Keycloak konfigürasyonu (Bölüm 5) ve fazlı yol haritası (Bölüm 7) bu dosyada.

Kurallar:
- .NET 9, klasik Program{Main} kalıbı, namespace HandCraft.*
- Kimlik: Keycloak (realm: handcraft), API'ler JWT Bearer ile doğrular (Bölüm 4 madde 4).
- Türkçe domain isimleri kullan (Katalog, Sepet, Siparis, Indirim, Odeme, Fotograf, Ortak).
- Her fazın sonunda "Kabul kriteri"ni karşıladığını derleyerek/çalıştırarak doğrula.
- Henüz kod yazma. Önce planı oku, solution yapısını ve kullanacağın NuGet paketlerini
  (referans repodaki sürümlerle: MediatR 13, MassTransit 8.5.5, MongoDB.Driver 3.5,
  StackExchange.Redis 2.9, Npgsql 10 + Dapper.Contrib 2, EF Core 9 SqlServer, FluentValidation 12)
  bana özetle ve onayımı bekle.
```

### 8.1–8.10 — Faz Promptları

**Faz 0:**
```
Faz 0'ı uygula (Solution iskeleti + HandCraft.Ortak + Docker altyapı + Keycloak realm).
Bölüm 2 (yapı), Bölüm 3 (portlar), Bölüm 4 (Ortak konvansiyonları) ve Bölüm 5'e (Keycloak)
harfiyen uy. Tüm projeleri oluştur (derlensin ama içleri boş olabilir), HandCraft.Ortak'ı
tamamla, tüm docker-compose dosyalarını + docker-compose.all.yml'i yaz, Keycloak için
realm-export.json üret. Sonunda `dotnet build` ve `docker compose -f infra/docker-compose.all.yml
config` çıktısını göster. Kabul kriterini doğrula.
```

**Faz 1:**
```
Faz 1'i uygula (Katalog — MongoDB). Bölüm 7/Faz 1 ve Bölüm 4 madde 8'e uy. Urun+Kategori
modelleri, servisler (IOptions<MongoDbSettings>+IMongoCollection), AutoMapper, controller'lar
(okuma public, yazma admin rolü), 10-12 el-işi ürün + kategori seed. Çalıştırıp Swagger'dan
test et, mongo-express'te koleksiyonları doğrula.
```

**Faz 2:**
```
Faz 2'yi uygula (Fotograf API). Multipart görsel yükleme → wwwroot/images → URL döndür,
JWT admin. Katalog ürünleri bu URL'yi kullansın.
```

**Faz 3:**
```
Faz 3'ü uygula (Indirim — PostgreSQL + Dapper, EF Core DEĞİL). Bölüm 4 madde 10'a uy.
NpgsqlConnection + Dapper ham SQL, açılışta `indirim` tablosunu oluşturan SQL, CRUD controller.
pgAdmin'de tabloyu doğrula.
```

**Faz 4:**
```
Faz 4'ü uygula (Sepet — Redis). Bölüm 4 madde 9'a uy. RedisService (singleton) + SepetService,
key sepet:{userId}, getir/ekle/sil/temizle, UserId token'dan (IIdentityHelperService).
redis-commander'da anahtarı doğrula.
```

**Faz 5:**
```
Faz 5'i uygula (Siparis — Clean Architecture + CQRS + MSSQL). Bölüm 7/Faz 5 ve Bölüm 4
madde 12-13'e uy. Domain/Persistence/Application/Api ayrımı net olsun; bağımlılık yönü:
Api→Application→(Domain,Persistence), Persistence→Domain. MediatR Command+Query handler'ları
GERÇEKTEN çalışsın (DB'ye yaz/oku). EF migration üret, `dotnet ef database update` çalıştır,
POST/GET'i test et.
```

**Faz 6:**
```
Faz 6'yı uygula (Odeme + RabbitMQ). Bölüm 6 akışına uy. Odeme publish → queue:
siparis-olustur-service; Siparis.Application'da SiparisOlusturConsumer mesajı MediatR
CreateSiparisCommand'e çevirip siparişi kaydetsin. Uçtan uca test: Odeme isteği → RabbitMQ
kuyruğu → MSSQL'de yeni sipariş.
```

**Faz 7:**
```
Faz 7'yi uygula (API Gateway — YARP). Bölüm 3 port planına göre tüm servislere route,
Bearer pass-through, CORS. Tüm API çağrıları artık 7000 üzerinden gitsin.
```

**Faz 8:**
```
Faz 8'i uygula (Public site — HandCraft.Web, MVC). Keycloak OIDC (web client) login,
ürün vitrini/liste/detay, sepet, checkout, siparişlerim. Backend çağrıları SADECE Gateway
(7000) üzerinden, access token Bearer iletilsin. Razor + Bootstrap, el-işi teması. Uçtan
uca alışveriş senaryosunu test et.
```

**Faz 9:**
```
Faz 9'u uygula (Admin — HandCraft.Admin, MVC). Keycloak OIDC + admin rol kontrolü.
Ürün/Kategori/İndirim CRUD + Fotograf yükleme + sipariş listesi. Gateway üzerinden.
customer rolünün admin'e giremediğini doğrula.
```

**Faz 10:**
```
Faz 10'u uygula (Entegrasyon + Doğrulama). RUN.md yaz (sıralı ayağa kaldırma + scriptler),
12 isteri tek tek ispatlayan demo senaryosu hazırla (Bölüm 1 tablosuna göre), smoke test ekle.
Sıfırdan tam akışı çalıştırıp Bölüm 1'deki tüm satırların ✅ olduğunu raporla.
```

---

## 9. Sık Karşılaşılacak Tuzaklar (Claude Code'a hatırlat)

- **Keycloak audience:** API `aud` claim'i client adıyla eşleşmezse 401 alırsın → realm'de audience mapper ekle.
- **`RoleClaimType="roles"`:** Keycloak rolleri varsayılan `realm_access.roles` içinde; token'a düz `roles` claim'i bindiren mapper gerekir. Aksi halde `[Authorize(Roles="admin")]` çalışmaz.
- **MassTransit kuyruk adı:** publisher'daki `queue:siparis-olustur-service` ile consumer endpoint adı eşleşmeli.
- **EF MigrationsAssembly:** migration'lar Persistence'ta ama Api'den çalıştırılıyorsa `MigrationsAssembly("HandCraft.Siparis.Persistence")` şart.
- **MSSQL container parolası:** güçlü parola kuralı (SA_PASSWORD) yoksa container açılmaz.
- **HTTPS redirect + Gateway:** geliştirmede `RequireHttpsMetadata=false`; gateway arkasında HTTPS redirect'i kapatmayı düşün.
- **Indirim'de EF değil Dapper:** yanlışlıkla EF Core eklenmesin (#6 ispatı için ilişkisel ama MSSQL-dışı + farklı erişim yöntemi olması güzel).
- **Docker port çakışması:** Keycloak Postgres (5432) ile Indirim Postgres (5433) ayrı kalsın.

---

## 10. Hocaya Sunum Notu (#7 için)

İster "IdentityServer4" yazıyor ama dersin kendi reposu **Keycloak** kullanıyor ve
IdentityServer4 2022'de kullanımdan kalktı (yerine ücretli Duende geçti). Bu projede,
**ders reposundaki ile aynı kimlik sunucusu yaklaşımı** (Keycloak, OIDC/JWT) uygulandı;
"merkezi kimlik sunucusu + token tabanlı doğrulama" gereksinimi bu şekilde karşılanıyor.
İstersen sunumdan önce hocaya tek satır teyit: *"IS4 EOL olduğu için ders reposundaki
Keycloak'ı kullandım, uygun mu?"*
```
```

---

### Bu planın kapsamadığı (bilerek) noktalar
- Üretim güvenliği (secret yönetimi, HTTPS sertifikaları) — ders/demo seviyesi yeterli.
- Kapsamlı unit/integration test — istersen Faz 10'a eklettirilebilir.
- Gerçek ödeme entegrasyonu — sahte ödeme (her zaman başarılı) yeterli.
