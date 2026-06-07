# HandCraft Shop - Tum .NET servislerini ayri pencerelerde baslatir.
# Onkosul: altyapi ayakta olmali -> docker compose -f infra/docker-compose.all.yml up -d
# Kullanim: ./run-all.ps1

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

# (proje yolu, port, etiket)
$servisler = @(
    @{ Path = "services/HandCraft.Katalog";                      Port = 7001; Ad = "Katalog"  },
    @{ Path = "services/HandCraft.Sepet";                        Port = 7002; Ad = "Sepet"    },
    @{ Path = "services/HandCraft.Siparis/HandCraft.Siparis.Api"; Port = 7003; Ad = "Siparis"  },
    @{ Path = "services/HandCraft.Indirim";                      Port = 7004; Ad = "Indirim"  },
    @{ Path = "services/HandCraft.Odeme";                        Port = 7005; Ad = "Odeme"    },
    @{ Path = "services/HandCraft.Fotograf";                     Port = 7006; Ad = "Fotograf" },
    @{ Path = "gateway/HandCraft.Gateway";                       Port = 7000; Ad = "Gateway"  },
    @{ Path = "web/HandCraft.Web";                               Port = 5000; Ad = "Web"      },
    @{ Path = "web/HandCraft.Admin";                             Port = 5001; Ad = "Admin"    }
)

Write-Host "HandCraft Shop servisleri baslatiliyor..." -ForegroundColor Cyan

foreach ($s in $servisler) {
    $full = Join-Path $root $s.Path
    Write-Host ("  -> {0,-10} (port {1})" -f $s.Ad, $s.Port) -ForegroundColor Green
    Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory $full -WindowStyle Minimized
    Start-Sleep -Seconds 2
}

Write-Host ""
Write-Host "Tum servisler baslatildi. Adresler:" -ForegroundColor Cyan
Write-Host "  Public site : http://localhost:5000"
Write-Host "  Admin panel : http://localhost:5001"
Write-Host "  Gateway     : http://localhost:7000"
Write-Host "  Keycloak    : http://localhost:8080 (admin/admin)"
Write-Host ""
Write-Host "Smoke test icin: ./smoke-test.ps1" -ForegroundColor Yellow
