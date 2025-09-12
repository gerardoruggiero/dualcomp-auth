using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
	public class CompanyPhone : Entity
	{
		public Guid CompanyId { get; private set; }
		public Guid PhoneTypeId { get; private set; }
		public string Phone { get; private set; } = string.Empty;
		public bool IsPrimary { get; private set; }

		private CompanyPhone() { }

		private CompanyPhone(Guid companyId, Guid phoneTypeId, string phone, bool isPrimary = false)
		{
			Id = Guid.NewGuid();
			CompanyId = companyId;
			PhoneTypeId = phoneTypeId == Guid.Empty ? throw new ArgumentException("PhoneTypeId cannot be empty", nameof(phoneTypeId)) : phoneTypeId;
			Phone = string.IsNullOrWhiteSpace(phone) ? throw new ArgumentException("Phone is required", nameof(phone)) : phone.Trim();
			IsPrimary = isPrimary;
		}

		public static CompanyPhone Create(Guid companyId, Guid phoneTypeId, string phone, bool isPrimary = false)
			=> new CompanyPhone(companyId, phoneTypeId, phone, isPrimary);

		public void UpdatePhone(string phone)
		{
			Phone = string.IsNullOrWhiteSpace(phone) ? throw new ArgumentException("Phone is required", nameof(phone)) : phone.Trim();
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
