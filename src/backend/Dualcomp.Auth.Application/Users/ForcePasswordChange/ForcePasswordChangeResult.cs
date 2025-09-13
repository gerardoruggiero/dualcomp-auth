namespace Dualcomp.Auth.Application.Users.ForcePasswordChange
{
    public class ForcePasswordChangeResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public static ForcePasswordChangeResult Success(string accessToken, string refreshToken, DateTime expiresAt)
        {
            return new ForcePasswordChangeResult
            {
                IsSuccess = true,
                Message = "Contraseña cambiada exitosamente. Ya puedes iniciar sesión normalmente.",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
        }

        public static ForcePasswordChangeResult Failure(string message)
        {
            return new ForcePasswordChangeResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}

