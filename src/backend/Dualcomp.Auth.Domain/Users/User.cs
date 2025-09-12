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
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
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
    }
}
