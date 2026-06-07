# HandCraft Shop - Smoke test: tum servisler ve altyapi ayakta mi?
# Kullanim: ./smoke-test.ps1

$ErrorActionPreference = "Continue"

function Test-Url($ad, $url, $beklenen = 200) {
    # Windows PowerShell 5.1 uyumlu: HTTP hata kodlari exception olarak gelir,
    # status'u Exception.Response'tan okuruz.
    $kod = $null
    try {
        # -MaximumRedirection 0: 302'yi takip etme (login redirect'ini oldugu gibi gor).
        # 5.1'de redirect bir hata olarak gelir; status'u Response'tan okuyup gurultuyu bastiririz.
        $r = Invoke-WebRequest -Uri $url -Method GET -TimeoutSec 5 -UseBasicParsing `
                -MaximumRedirection 0 -ErrorAction SilentlyContinue
        if ($r) { $kod = [int]$r.StatusCode }
    } catch {
        if ($_.Exception.Response) {
            $kod = [int]$_.Exception.Response.StatusCode
        }
    }

    if ($null -eq $kod) {
        Write-Host ("  [FAIL] {0,-22} {1} (baglanti yok)" -f $ad, $url) -ForegroundColor Red
    } elseif ($kod -eq $beklenen) {
        Write-Host ("  [OK]   {0,-22} {1} ({2})" -f $ad, $url, $kod) -ForegroundColor Green
    } else {
        Write-Host ("  [WARN] {0,-22} {1} ({2}, beklenen {3})" -f $ad, $url, $kod, $beklenen) -ForegroundColor Yellow
    }
}

Write-Host "=== Altyapi (Docker) ===" -ForegroundColor Cyan
Test-Url "Keycloak"        "http://localhost:8080/realms/handcraft/.well-known/openid-configuration"
Test-Url "mongo-express"   "http://localhost:8081"
Test-Url "redis-commander" "http://localhost:8082"
Test-Url "pgAdmin"         "http://localhost:8888/misc/ping"
Test-Url "RabbitMQ UI"     "http://localhost:15672"

Write-Host ""
Write-Host "=== Mikroservisler (Swagger) ===" -ForegroundColor Cyan
Test-Url "Katalog API"  "http://localhost:7001/swagger/v1/swagger.json"
Test-Url "Sepet API"    "http://localhost:7002/swagger/v1/swagger.json"
Test-Url "Siparis API"  "http://localhost:7003/swagger/v1/swagger.json"
Test-Url "Indirim API"  "http://localhost:7004/swagger/v1/swagger.json"
Test-Url "Odeme API"    "http://localhost:7005/swagger/v1/swagger.json"
Test-Url "Fotograf API" "http://localhost:7006/swagger/v1/swagger.json"

Write-Host ""
Write-Host "=== Gateway + Frontend ===" -ForegroundColor Cyan
Test-Url "Gateway->Katalog" "http://localhost:7000/katalog/api/urun"
Test-Url "Public site"      "http://localhost:5000/"
Test-Url "Admin (login)"    "http://localhost:5001/" 302

Write-Host ""
Write-Host "Smoke test tamamlandi." -ForegroundColor Cyan
