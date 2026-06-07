using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HandCraft.Ortak.Identity
{
    // HttpContext uzerinden claim okur. DI'da AddHttpContextAccessor() ile birlikte register edilir.
    public class IdentityHelperService : IIdentityHelperService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityHelperService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            // Keycloak 'sub' -> NameIdentifier'a maplenir.
            return user?.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? user?.FindFirstValue("sub")
                   ?? string.Empty;
        }

        public string GetUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirstValue("preferred_username")
                   ?? user?.Identity?.Name
                   ?? string.Empty;
        }
    }
}
