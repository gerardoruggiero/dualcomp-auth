namespace Dualcomp.Auth.Application.Users.Login
{
    public record LoginResult(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        Guid UserId,
        string Email,
        string FullName,
        Guid? CompanyId,
        bool IsCompanyAdmin,
        bool RequiresPasswordChange = false,
        bool IsEmailValidated = true
    );
}
