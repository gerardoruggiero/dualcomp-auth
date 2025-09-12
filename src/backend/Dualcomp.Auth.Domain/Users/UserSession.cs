using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Users
{
    public class UserSession : Entity
    {
        public Guid UserId { get; private set; }
        public string AccessToken { get; private set; } = string.Empty;
        public string RefreshToken { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? LastUsedAt { get; private set; }
        public bool IsActive { get; private set; }
        public string? UserAgent { get; private set; }
        public string? IpAddress { get; private set; }

        private UserSession() { }

        private UserSession(Guid userId, string accessToken, string refreshToken, DateTime expiresAt, string? userAgent = null, string? ipAddress = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            IsActive = true;
            UserAgent = userAgent;
            IpAddress = ipAddress;
        }

        public static UserSession Create(Guid userId, string accessToken, string refreshToken, DateTime expiresAt, string? userAgent = null, string? ipAddress = null)
            => new UserSession(userId, accessToken, refreshToken, expiresAt, userAgent, ipAddress);

        public void UpdateTokens(string accessToken, string refreshToken, DateTime expiresAt)
        {
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
            ExpiresAt = expiresAt;
            LastUsedAt = DateTime.UtcNow;
        }

        public void MarkAsUsed()
        {
            LastUsedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }
    }
}
