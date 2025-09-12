using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
	public class CompanyAddress : Entity
	{
		public Guid CompanyId { get; private set; }
		public Guid AddressTypeId { get; private set; }
		public string Address { get; private set; } = string.Empty;
		public bool IsPrimary { get; private set; }

		private CompanyAddress() { }

		private CompanyAddress(Guid companyId, Guid addressTypeId, string address, bool isPrimary = false)
		{
			Id = Guid.NewGuid();
			CompanyId = companyId;
			AddressTypeId = addressTypeId == Guid.Empty ? throw new ArgumentException("AddressTypeId cannot be empty", nameof(addressTypeId)) : addressTypeId;
			Address = string.IsNullOrWhiteSpace(address) ? throw new ArgumentException("Address is required", nameof(address)) : address.Trim();
			IsPrimary = isPrimary;
		}

		public static CompanyAddress Create(Guid companyId, Guid addressTypeId, string address, bool isPrimary = false)
			=> new CompanyAddress(companyId, addressTypeId, address, isPrimary);

		public void UpdateAddress(string address)
		{
			Address = string.IsNullOrWhiteSpace(address) ? throw new ArgumentException("Address is required", nameof(address)) : address.Trim();
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
