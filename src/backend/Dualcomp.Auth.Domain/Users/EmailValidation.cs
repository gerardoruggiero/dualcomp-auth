using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Users
{
    public class EmailValidation : Entity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }

        private EmailValidation() { }

        private EmailValidation(Guid userId, string token, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Token = string.IsNullOrWhiteSpace(token) ? throw new ArgumentException("Token is required", nameof(token)) : token;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            IsUsed = false;
        }

        public static EmailValidation Create(Guid userId, string token, DateTime expiresAt)
            => new EmailValidation(userId, token, expiresAt);

        public static EmailValidation CreateWithDefaultExpiration(Guid userId, string token)
        {
            // Token expira en 24 horas por defecto
            var expiresAt = DateTime.UtcNow.AddHours(24);
            return new EmailValidation(userId, token, expiresAt);
        }

        public void MarkAsUsed()
        {
            if (IsUsed)
                throw new InvalidOperationException("Email validation token has already been used");

            if (IsExpired())
                throw new InvalidOperationException("Email validation token has expired");

            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }

        public bool IsValid()
        {
            return !IsUsed && !IsExpired();
        }

        public TimeSpan GetTimeUntilExpiration()
        {
            return ExpiresAt - DateTime.UtcNow;
        }
    }
}
