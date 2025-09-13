using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Users
{
    public class User : AggregateRoot
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public HashedPassword Password { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public bool IsEmailValidated { get; private set; }
        public bool MustChangePassword { get; private set; }
        public string? TemporaryPassword { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public DateTime? EmailValidatedAt { get; private set; }
        public Guid? CompanyId { get; private set; }
        public bool IsCompanyAdmin { get; private set; }

        private User() { }

        private User(string firstName, string lastName, Email email, HashedPassword password, Guid? companyId = null, bool isCompanyAdmin = false)
        {
            Id = Guid.NewGuid();
            FirstName = string.IsNullOrWhiteSpace(firstName) ? throw new ArgumentException("FirstName is required", nameof(firstName)) : firstName.Trim();
            LastName = string.IsNullOrWhiteSpace(lastName) ? throw new ArgumentException("LastName is required", nameof(lastName)) : lastName.Trim();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            IsActive = true;
            IsEmailValidated = false;
            MustChangePassword = false;
            TemporaryPassword = null;
            CreatedAt = DateTime.UtcNow;
            CompanyId = companyId;
            IsCompanyAdmin = isCompanyAdmin;
        }

        public static User Create(string firstName, string lastName, Email email, HashedPassword password, Guid? companyId = null, bool isCompanyAdmin = false)
            => new User(firstName, lastName, email, password, companyId, isCompanyAdmin);

        public void UpdatePassword(HashedPassword newPassword)
        {
            Password = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
        }

        public void UpdateProfile(string firstName, string lastName, Email email)
        {
            FirstName = string.IsNullOrWhiteSpace(firstName) ? throw new ArgumentException("FirstName is required", nameof(firstName)) : firstName.Trim();
            LastName = string.IsNullOrWhiteSpace(lastName) ? throw new ArgumentException("LastName is required", nameof(lastName)) : lastName.Trim();
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        public void SetLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public string GetFullName() => $"{FirstName} {LastName}".Trim();

        public void ValidateEmail()
        {
            if (IsEmailValidated)
                throw new InvalidOperationException("Email has already been validated");

            IsEmailValidated = true;
            EmailValidatedAt = DateTime.UtcNow;
            
            // Si el usuario no está activo, activarlo al validar el email
            if (!IsActive)
            {
                IsActive = true;
            }
        }

        public void SetMustChangePassword(bool mustChange = true)
        {
            MustChangePassword = mustChange;
        }

        public void SetTemporaryPassword(string temporaryPassword)
        {
            TemporaryPassword = string.IsNullOrWhiteSpace(temporaryPassword) 
                ? throw new ArgumentException("Temporary password is required", nameof(temporaryPassword)) 
                : temporaryPassword;
            MustChangePassword = true;
        }

        public void ClearTemporaryPassword()
        {
            TemporaryPassword = null;
            MustChangePassword = false;
        }

        public bool CanLogin()
        {
            return IsActive && IsEmailValidated;
        }

        public bool RequiresPasswordChange()
        {
            return MustChangePassword;
        }

        public bool HasValidTemporaryPassword(string providedTemporaryPassword)
        {
            return !string.IsNullOrWhiteSpace(TemporaryPassword) && 
                   TemporaryPassword.Equals(providedTemporaryPassword, StringComparison.Ordinal);
        }

        public void SetCompanyAdmin(bool isAdmin = true)
        {
            IsCompanyAdmin = isAdmin;
        }
    }
}
