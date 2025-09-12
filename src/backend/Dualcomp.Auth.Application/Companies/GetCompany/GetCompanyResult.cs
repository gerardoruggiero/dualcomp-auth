namespace Dualcomp.Auth.Application.Companies.GetCompany
{
    public class GetCompanyResult(
        Guid Id,
        string Name,
        string TaxId,
        List<CompanyAddressResult> Addresses,
        List<CompanyEmailResult> Emails,
        List<CompanyPhoneResult> Phones,
        List<CompanySocialMediaResult> SocialMedias,
        List<CompanyEmployeeResult> Employees
    );

    public record CompanyAddressResult(
        string AddressType,
        string Address,
        bool IsPrimary
    );

    public record CompanyEmailResult(
        string EmailType,
        string Email,
        bool IsPrimary
    );

    public record CompanyPhoneResult(
        string PhoneType,
        string Phone,
        bool IsPrimary
    );

    public record CompanySocialMediaResult(
        string SocialMediaType,
        string Url,
        bool IsPrimary
    );

    public record CompanyEmployeeResult(
        string FullName,
        string Email,
        string? Phone,
        string? Position,
        DateTime? HireDate
    );
}
