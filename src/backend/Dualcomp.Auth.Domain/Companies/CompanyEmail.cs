using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
	public class CompanyEmail : Entity
	{
		public Guid CompanyId { get; private set; }
		public Guid EmailTypeId { get; private set; }
		public Email Email { get; private set; } = null!;
		public bool IsPrimary { get; private set; }

		private CompanyEmail() { }

		private CompanyEmail(Guid companyId, Guid emailTypeId, Email email, bool isPrimary = false)
		{
			Id = Guid.NewGuid();
			CompanyId = companyId;
			EmailTypeId = emailTypeId == Guid.Empty ? throw new ArgumentException("EmailTypeId cannot be empty", nameof(emailTypeId)) : emailTypeId;
			Email = email ?? throw new ArgumentNullException(nameof(email));
			IsPrimary = isPrimary;
		}

		public static CompanyEmail Create(Guid companyId, Guid emailTypeId, Email email, bool isPrimary = false)
			=> new CompanyEmail(companyId, emailTypeId, email, isPrimary);

		public void UpdateEmail(Email email)
		{
			Email = email ?? throw new ArgumentNullException(nameof(email));
		}

		public void SetAsPrimary()
		{
			IsPrimary = true;
		}

		public void SetAsSecondary()
		{
			IsPrimary = false;
		}
	}
}
