namespace Dualcomp.Auth.Application.Users.RefreshToken
{
    public record RefreshTokenResult(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        Guid UserId,
        string Email,
        string FullName,
        Guid? CompanyId,
        bool IsCompanyAdmin
    );
}
