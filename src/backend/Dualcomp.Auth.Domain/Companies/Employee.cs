using Dualcomp.Auth.Domain.Users;
using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
	public class Employee : Entity
	{
		public string FullName { get; private set; } = string.Empty;
		public string Email { get; private set; } = string.Empty;
		public string? Phone { get; private set; }
		public Guid CompanyId { get; private set; }
		public Guid? UserId { get; private set; }
		public string? Position { get; private set; }
		public DateTime? HireDate { get; private set; }
		public bool IsActive { get; private set; }
        public User User { get; private set; } = null!;

        private Employee() { }

        private Employee(string fullName, string email, string? phone, Guid companyId, string? position = null, DateTime? hireDate = null, User user = null)
        {
            Id = Guid.NewGuid();
            FullName = string.IsNullOrWhiteSpace(fullName) ? throw new ArgumentException("FullName is required", nameof(fullName)) : fullName.Trim();

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            var trimmedEmail = email.Trim();
            if (!IsValidEmail(trimmedEmail))
                throw new ArgumentException("Invalid email format", nameof(email));

            Email = trimmedEmail;
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            CompanyId = companyId;
            Position = string.IsNullOrWhiteSpace(position) ? null : position.Trim();
            HireDate = hireDate ?? DateTime.UtcNow;
            User = user;
			UserId = user.Id;
            IsActive = true;
        }

        private Employee(string fullName, string email, string? phone, Guid companyId, string? position = null, DateTime? hireDate = null, Guid? userId = null)
		{
			Id = Guid.NewGuid();
			FullName = string.IsNullOrWhiteSpace(fullName) ? throw new ArgumentException("FullName is required", nameof(fullName)) : fullName.Trim();
			
			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email is required", nameof(email));
			
			var trimmedEmail = email.Trim();
			if (!IsValidEmail(trimmedEmail))
				throw new ArgumentException("Invalid email format", nameof(email));
			
			Email = trimmedEmail;
			Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
			CompanyId = companyId;
			Position = string.IsNullOrWhiteSpace(position) ? null : position.Trim();
			HireDate = hireDate ?? DateTime.UtcNow;
			UserId = userId;
			IsActive = true;
		}
        public static Employee Create(string fullName, string email, string? phone, Guid companyId, string? position = null, DateTime? hireDate = null, User? user = null)
            => new Employee(fullName, email, phone, companyId, position, hireDate, user);

        public static Employee Create(string fullName, string email, string? phone, Guid companyId, string? position = null, DateTime? hireDate = null, Guid? userId = null)
			=> new Employee(fullName, email, phone, companyId, position, hireDate, userId);

		public void UpdateProfile(string fullName, string email, string? phone, string? position)
		{
			FullName = string.IsNullOrWhiteSpace(fullName) ? throw new ArgumentException("FullName is required", nameof(fullName)) : fullName.Trim();
			
			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email is required", nameof(email));
			
			var trimmedEmail = email.Trim();
			if (!IsValidEmail(trimmedEmail))
				throw new ArgumentException("Invalid email format", nameof(email));
			
			Email = trimmedEmail;
			Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
			Position = string.IsNullOrWhiteSpace(position) ? null : position.Trim();
		}

		private static bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		public void AssignUser(Guid userId)
		{
			UserId = userId;
		}

		public void UnassignUser()
		{
			UserId = null;
		}

		public void Deactivate()
		{
			IsActive = false;
		}

		public void Activate()
		{
			IsActive = true;
		}
	}
}
