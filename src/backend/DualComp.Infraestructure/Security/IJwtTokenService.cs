using System.Security.Claims;

namespace DualComp.Infraestructure.Security
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Guid userId, string email, Guid? companyId, Guid sessionId, bool isCompanyAdmin);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        DateTime GetTokenExpiration(string token);
    }
}
