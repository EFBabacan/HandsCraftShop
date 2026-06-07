# HandCraft Shop — Keycloak post-import kurulum (idempotent)
# ----------------------------------------------------------------------------
# NEDEN: Keycloak 26'nin `start-dev --import-realm` ozelligi, realm-export.json
# icindeki `clientScopes` dizisinde ILK scope disindakileri sessizce ATLIYOR
# (bilinen import sinirlamasi). Bu yuzden 'username-flat' ve 'sub-flat' scope'lari
# import sonrasi admin API ile garanti edilir ve tum client'lara default atanir.
#
# KULLANIM (Keycloak ayaktayken):
#   pwsh -File infra/keycloak/setup-keycloak.ps1
# Tekrar tekrar calistirilabilir; var olani bozmaz.
# ----------------------------------------------------------------------------

param(
    [string]$KeycloakUrl = "http://localhost:8080",
    [string]$Realm       = "handcraft",
    [string]$AdminUser   = "admin",
    [string]$AdminPass   = "admin"
)

$ErrorActionPreference = "Stop"

Write-Host "[setup-keycloak] Admin token aliniyor..."
$tokenBody = @{ grant_type = "password"; client_id = "admin-cli"; username = $AdminUser; password = $AdminPass }
$at = (Invoke-RestMethod -Method Post -Uri "$KeycloakUrl/realms/master/protocol/openid-connect/token" -Body $tokenBody).access_token
$headers = @{ Authorization = "Bearer $at"; "Content-Type" = "application/json" }

function Get-Scopes { Invoke-RestMethod -Uri "$KeycloakUrl/admin/realms/$Realm/client-scopes" -Headers $headers }

# --- Eksik scope'lari olustur ---
$wanted = @(
    @{
        name = "username-flat"
        mapper = @{
            name = "preferred-username-flat"; protocol = "openid-connect"
            protocolMapper = "oidc-usermodel-property-mapper"; consentRequired = $false
            config = @{ "user.attribute" = "username"; "claim.name" = "preferred_username"; "jsonType.label" = "String"; "id.token.claim" = "true"; "access.token.claim" = "true"; "userinfo.token.claim" = "true" }
        }
    },
    @{
        name = "sub-flat"
        mapper = @{
            name = "subject-id"; protocol = "openid-connect"
            protocolMapper = "oidc-sub-mapper"; consentRequired = $false
            config = @{ "id.token.claim" = "true"; "access.token.claim" = "true"; "introspection.token.claim" = "true" }
        }
    }
)

$existing = Get-Scopes
foreach ($w in $wanted) {
    if ($existing.name -contains $w.name) {
        Write-Host "[setup-keycloak] scope '$($w.name)' zaten var, atlandi."
        continue
    }
    $body = @{
        name = $w.name; protocol = "openid-connect"
        attributes = @{ "include.in.token.scope" = "true"; "display.on.consent.screen" = "false" }
        protocolMappers = @($w.mapper)
    } | ConvertTo-Json -Depth 10
    Invoke-RestMethod -Method Post -Uri "$KeycloakUrl/admin/realms/$Realm/client-scopes" -Headers $headers -Body $body | Out-Null
    Write-Host "[setup-keycloak] scope '$($w.name)' olusturuldu."
}

# --- Tum client'lara default scope olarak ata ---
$scopes = Get-Scopes
$clients = Invoke-RestMethod -Uri "$KeycloakUrl/admin/realms/$Realm/clients" -Headers $headers
$targetClients = @("web", "admin", "katalog", "sepet", "siparis", "indirim", "odeme", "fotograf")

foreach ($scopeName in @("username-flat", "sub-flat")) {
    $scope = $scopes | Where-Object { $_.name -eq $scopeName }
    foreach ($cid in $targetClients) {
        $client = $clients | Where-Object { $_.clientId -eq $cid }
        if ($null -eq $client) { continue }
        Invoke-RestMethod -Method Put `
            -Uri "$KeycloakUrl/admin/realms/$Realm/clients/$($client.id)/default-client-scopes/$($scope.id)" `
            -Headers $headers | Out-Null
    }
    Write-Host "[setup-keycloak] '$scopeName' tum client'lara default atandi."
}

Write-Host "[setup-keycloak] TAMAM. preferred_username + sub artik token'larda."
