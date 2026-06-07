# HandCraft Shop — Çalıştırma ve Demo Kılavuzu

El-işi ürün satışı yapan mikroservis e-ticaret platformu. Mikroservis Mimarisi dersinin
12 isterini karşılar. Bu doküman sıfırdan ayağa kaldırmayı, smoke testi ve hocaya sunulacak
12 isterin tek tek ispatını içerir.

---

## 1. Önkoşullar

- .NET 9 SDK
- Docker Desktop
- `dotnet-ef` aracı: `dotnet tool install --global dotnet-ef --version 9.0.16`

---

## 2. Sıralı Ayağa Kaldırma

### Adım 1 — Altyapı (Docker)

```powershell
docker compose -f infra/docker-compose.all.yml up -d
```

Bu komut şunları başlatır: Keycloak (8080) + Postgres, MongoDB (27017) + mongo-express (8081),
Redis (6379) + redis-commander (8082), Indirim PostgreSQL (5433) + pgAdmin (8888),
RabbitMQ (5672) + management UI (15672), MS SQL Server (1433).

Keycloak ilk açılışta `infra/keycloak/realm-export.json`'u import eder → `handcraft` realm'i,
client'lar, roller, test kullanıcıları otomatik gelir. Keycloak'ın açılması ~30-60 sn sürebilir.

### Adım 2 — Veritabanı migration (Siparis / MSSQL)

```powershell
dotnet ef database update `
  --project services/HandCraft.Siparis/HandCraft.Siparis.Persistence `
  --startup-project services/HandCraft.Siparis/HandCraft.Siparis.Api
```

(Katalog/Mongo seed'i ilk çalıştırmada otomatik; Indirim tablosu uygulama açılışında oluşur.)

### Adım 3 — Servisleri başlat

Tek komutla (ayrı pencereler):

```powershell
./run-all.ps1
```

veya her birini elle: `dotnet run` (ilgili proje klasöründe). Sıra: önce 6 API + Gateway,
sonra Web ve Admin.

### Adım 4 — Smoke test

```powershell
./smoke-test.ps1
```

Tüm satırlar `[OK]` olmalı.

---

## 3. Adresler

| Bileşen | URL | Not |
|---------|-----|-----|
| Public site | http://localhost:5000 | Müşteri |
| Admin panel | http://localhost:5001 | Yönetici |
| Gateway | http://localhost:7000 | Tek API girişi |
| Keycloak | http://localhost:8080 | admin / admin |
| mongo-express | http://localhost:8081 | — |
| redis-commander | http://localhost:8082 | — |
| pgAdmin | http://localhost:8888 | admin@handcraft.com / admin |
| RabbitMQ UI | http://localhost:15672 | guest / guest |

**Test kullanıcıları:** `musteri1 / musteri1` (customer), `admin1 / admin1` (admin)

---

## 4. Port Planı

| Servis | Port | Servis | Port |
|--------|------|--------|------|
| Gateway | 7000 | Indirim | 7004 |
| Katalog | 7001 | Odeme | 7005 |
| Sepet | 7002 | Fotograf | 7006 |
| Siparis | 7003 | Web / Admin | 5000 / 5001 |

---

## 5. Demo Senaryosu — 12 İsterin Tek Tek İspatı

> Hocaya sunum sırası. Her madde, ilgili isterin **çalıştığını** gösterir.

### #1 — .NET 9 web uygulaması çalışıyor
`./smoke-test.ps1` → tüm servisler ayakta. Herhangi bir API'nin `/swagger` sayfasını aç
(örn. http://localhost:7001/swagger).

### #2 — Public site (MVC)
http://localhost:5000 → el-işi ürün vitrini. Kategori filtrele, ürün detayına gir.

### #3 — Admin yönetimi (MVC)
http://localhost:5001 → `admin1` ile giriş → ürün/kategori/indirim CRUD, sipariş listesi.

### #4 — MS SQL Server
`admin1` veya `musteri1` sipariş verince `handcraft_siparis` veritabanına yazılır.
Doğrulama (SSMS / Azure Data Studio / sqlcmd):
```
SELECT * FROM Siparisler; SELECT * FROM SiparisUrunBilgileri;
```
EF Core + SqlServer, Clean Architecture (Domain/Persistence/Application/Api).

### #5 — NoSQL (MongoDB)
Katalog servisi ürün/kategoriyi MongoDB'de tutar. mongo-express (8081) →
`handcraft_katalog` veritabanı → `urun` / `kategori` koleksiyonları.

### #6 — MSSQL harici ilişkisel (PostgreSQL + Dapper)
Indirim servisi PostgreSQL'de (5433), **Dapper + ham SQL** (EF Core değil).
pgAdmin (8888) → `indirim` veritabanı → `indirim` tablosu.

### #7 — Kimlik sunucusu (Keycloak)
http://localhost:8080 → `handcraft` realm → client'lar (web, admin, katalog, sepet, ...),
roller (customer, admin), kullanıcılar. Login OIDC akışı + JWT Bearer doğrulama her API'de.
> Not: İster "IdentityServer4" diyor; IS4 2022'de EOL olduğu için ders reposundaki gibi
> Keycloak kullanıldı (aynı merkezi kimlik + token doğrulama yaklaşımı).

### #8 — Redis
Sepet servisi sepeti Redis'te `sepet:{userId}` anahtarında tutar.
Sitede sepete ürün ekle → redis-commander (8082) → anahtarı gör.

### #9 — CQRS (MediatR)
Siparis.Application: `Features/Commands/CreateSiparis` + `Features/Queries/GetSiparislerByUserId`
(+ `GetTumSiparisler`). `IRequest` / `IRequestHandler` ayrımı.

### #10 — RabbitMQ (MassTransit)
Checkout'ta Odeme servisi `queue:siparis-olustur-service`'e mesaj publish eder;
Siparis.Application'daki `SiparisOlusturConsumer` tüketip siparişi MSSQL'e yazar.
RabbitMQ UI (15672) → Queues → `siparis-olustur-service` (mesaj hareketi).

### #11 — Clean / Onion mimari
Siparis 4 katman: `Domain` ← `Persistence`, `Application` → (Domain, Persistence),
`Api` → Application. Bağımlılıklar içe doğru.

### #12 — API Gateway (YARP)
Tüm backend çağrıları http://localhost:7000 üzerinden route'lanır
(`/katalog/**`→7001, `/sepet/**`→7002, ...). Web ve Admin yalnızca Gateway'i çağırır.

---

## 6. Uçtan Uca Alışveriş Akışı (canlı demo)

1. http://localhost:5000 → **Giriş Yap** → Keycloak → `musteri1 / musteri1`
2. Vitrinden ürün → **Sepete Ekle** (Redis'e yazılır — #8)
3. **Sepetim** → **Ödemeye Geç** → adres + kart → **Ödemeyi Tamamla**
4. Odeme → RabbitMQ → Siparis consumer → MSSQL (#10, #4)
5. **Siparişlerim** → yeni sipariş görünür
6. http://localhost:5001 → `admin1` ile → **Siparişler** → aynı sipariş admin panelinde

---

## 7. Kapatma

```powershell
# .NET servisleri: acilan pencereleri kapat veya:
Get-Process dotnet | Stop-Process -Force
# Altyapi:
docker compose -f infra/docker-compose.all.yml down
```
