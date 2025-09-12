using Dualcomp.Auth.Application.Companies.GetCompany;

namespace Dualcomp.Auth.Application.Companies.RegisterCompany
{
	/// <summary>
	/// Resultado del registro de empresa - reutiliza tipos de GetCompany para consistencia
	/// </summary>
	public class RegisterCompanyResult
	{
		public Guid CompanyId { get; init; }
		public string Name { get; init; } = string.Empty;
		public string TaxId { get; init; } = string.Empty;
		public List<GetCompany.CompanyAddressResult> Addresses { get; init; } = [];
		public List<GetCompany.CompanyEmailResult> Emails { get; init; } = [];
		public List<GetCompany.CompanyPhoneResult> Phones { get; init; } = [];
		public List<GetCompany.CompanySocialMediaResult> SocialMedias { get; init; } = [];
		public List<GetCompany.CompanyEmployeeResult> Employees { get; init; } = [];

		public RegisterCompanyResult(
			Guid companyId,
			string name,
			string taxId,
			List<GetCompany.CompanyAddressResult> addresses,
			List<GetCompany.CompanyEmailResult> emails,
			List<GetCompany.CompanyPhoneResult> phones,
			List<GetCompany.CompanySocialMediaResult> socialMedias,
			List<GetCompany.CompanyEmployeeResult> employees)
		{
			CompanyId = companyId;
			Name = name;
			TaxId = taxId;
			Addresses = addresses;
			Emails = emails;
			Phones = phones;
			SocialMedias = socialMedias;
			Employees = employees;
		}
	}
}
