using System.Security.Claims;

namespace Dualcomp.Auth.WebApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public static Guid GetUserIdOrThrow(this ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }
            return userId.Value;
        }

        public static Guid? GetCompanyId(this ClaimsPrincipal user)
        {
            // El claim en JwtTokenService es "companyId" (minúscula)
            var companyIdClaim = user.FindFirst("companyId")?.Value;

            if (string.IsNullOrEmpty(companyIdClaim))
                return null;

            return Guid.TryParse(companyIdClaim, out var companyId) ? companyId : null;
        }

        public static Guid GetCompanyIdOrThrow(this ClaimsPrincipal user)
        {
            var companyId = user.GetCompanyId();
            if (companyId == null)
            {
                throw new UnauthorizedAccessException("Empresa no identificada");
            }
            return companyId.Value;
        }

        public static bool IsCompanyAdmin(this ClaimsPrincipal user)
        {
            var isAdminClaim = user.FindFirst("isCompanyAdmin")?.Value;
            return bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin;
        }
    }
}
