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
		public string AddressType { get; init; } = string.Empty;
		public string Address { get; init; } = string.Empty;
		public bool IsPrimary { get; init; } = false;
	}

	public class RegisterCompanyEmailDto
	{
		public string EmailType { get; init; } = string.Empty;
		public string Email { get; init; } = string.Empty;
		public bool IsPrimary { get; init; } = false;
	}

	public class RegisterCompanyPhoneDto
	{
		public string PhoneType { get; init; } = string.Empty;
		public string Phone { get; init; } = string.Empty;
		public bool IsPrimary { get; init; } = false;
	}

	public class RegisterCompanySocialMediaDto
	{
		public string SocialMediaType { get; init; } = string.Empty;
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
