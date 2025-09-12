using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Companies.RegisterCompany
{
	public class RegisterCompanyCommand : ICommand<RegisterCompanyResult>
	{
		public string Name { get; init; } = string.Empty;
		public string TaxId { get; init; } = string.Empty;
		public List<RegisterCompanyAddressDto> Addresses { get; init; } = [];
		public List<RegisterCompanyEmailDto> Emails { get; init; } = [];
		public List<RegisterCompanyPhoneDto> Phones { get; init; } = [];
		public List<RegisterCompanySocialMediaDto> SocialMedias { get; init; } = [];
		public List<RegisterCompanyEmployeeDto> Employees { get; init; } = [];
	}

	public class RegisterCompanyAddressDto
	{
		public Guid AddressTypeId { get; init; }
		public string Address { get; init; } = string.Empty;
		public bool IsPrimary { get; init; } = false;
	}

	public class RegisterCompanyEmailDto
	{
		public Guid EmailTypeId { get; init; }
		public string Email { get; init; } = string.Empty;
		public bool IsPrimary { get; init; } = false;
	}

	public class RegisterCompanyPhoneDto
	{
		public Guid PhoneTypeId { get; init; }
		public string Phone { get; init; } = string.Empty;
		public bool IsPrimary { get; init; } = false;
	}

	public class RegisterCompanySocialMediaDto
	{
		public Guid SocialMediaTypeId { get; init; }
		public string Url { get; init; } = string.Empty;
		public bool IsPrimary { get; init; } = false;
	}

	public class RegisterCompanyEmployeeDto
	{
		public string FullName { get; init; } = string.Empty;
		public string Email { get; init; } = string.Empty;
		public string? Phone { get; init; }
		public string? Position { get; init; }
		public DateTime? HireDate { get; init; }
	}
}
