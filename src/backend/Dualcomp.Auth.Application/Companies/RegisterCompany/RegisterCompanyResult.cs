namespace Dualcomp.Auth.Application.Companies.RegisterCompany
{
	public class RegisterCompanyResult
	{
		public Guid CompanyId { get; init; }
		public string Name { get; init; } = string.Empty;
		public string TaxId { get; init; } = string.Empty;
		public List<CompanyAddressResult> Addresses { get; init; } = [];
		public List<CompanyEmailResult> Emails { get; init; } = [];
		public List<CompanyPhoneResult> Phones { get; init; } = [];
		public List<CompanySocialMediaResult> SocialMedias { get; init; } = [];
		public List<CompanyEmployeeResult> Employees { get; init; } = [];
	}

	public class CompanyAddressResult
	{
		public string AddressType { get; init; } = string.Empty;
		public string Address { get; init; } = string.Empty;
		public bool IsPrimary { get; init; }

		public CompanyAddressResult(string addressType, string address, bool isPrimary)
		{
			AddressType = addressType;
			Address = address;
			IsPrimary = isPrimary;
		}
	}

	public class CompanyEmailResult
	{
		public string EmailType { get; init; } = string.Empty;
		public string Email { get; init; } = string.Empty;
		public bool IsPrimary { get; init; }

		public CompanyEmailResult(string emailType, string email, bool isPrimary)
		{
			EmailType = emailType;
			Email = email;
			IsPrimary = isPrimary;
		}
	}

	public class CompanyPhoneResult
	{
		public string PhoneType { get; init; } = string.Empty;
		public string Phone { get; init; } = string.Empty;
		public bool IsPrimary { get; init; }

		public CompanyPhoneResult(string phoneType, string phone, bool isPrimary)
		{
			PhoneType = phoneType;
			Phone = phone;
			IsPrimary = isPrimary;
		}
	}

	public class CompanySocialMediaResult
	{
		public string SocialMediaType { get; init; } = string.Empty;
		public string Url { get; init; } = string.Empty;
		public bool IsPrimary { get; init; }

		public CompanySocialMediaResult(string socialMediaType, string url, bool isPrimary)
		{
			SocialMediaType = socialMediaType;
			Url = url;
			IsPrimary = isPrimary;
		}
	}

	public class CompanyEmployeeResult
	{
		public string FullName { get; init; } = string.Empty;
		public string Email { get; init; } = string.Empty;
		public string? Phone { get; init; }
		public string? Position { get; init; }
		public DateTime? HireDate { get; init; }

		public CompanyEmployeeResult(string fullName, string email, string? phone, string? position, DateTime? hireDate)
		{
			FullName = fullName;
			Email = email;
			Phone = phone;
			Position = position;
			HireDate = hireDate;
		}
	}
}
