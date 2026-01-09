using System.Text.Json.Serialization;

namespace Dualcomp.Auth.Application.Companies.GetCompany
{
    public record GetCompanyResult(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("taxId")] string TaxId,
        [property: JsonPropertyName("addresses")] List<CompanyAddressResult> Addresses,
        [property: JsonPropertyName("emails")] List<CompanyEmailResult> Emails,
        [property: JsonPropertyName("phones")] List<CompanyPhoneResult> Phones,
        [property: JsonPropertyName("socialMedias")] List<CompanySocialMediaResult> SocialMedias,
        [property: JsonPropertyName("employees")] List<CompanyEmployeeResult> Employees,
        [property: JsonPropertyName("moduleIds")] List<Guid> ModuleIds
    );

    public record CompanyAddressResult(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("addressType")] string AddressType,
        [property: JsonPropertyName("address")] string Address,
        [property: JsonPropertyName("isPrimary")] bool IsPrimary
    );

    public record CompanyEmailResult(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("emailType")] string EmailType,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("isPrimary")] bool IsPrimary
    );

    public record CompanyPhoneResult(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("phoneType")] string PhoneType,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("isPrimary")] bool IsPrimary
    );

    public record CompanySocialMediaResult(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("socialMediaType")] string SocialMediaType,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("isPrimary")] bool IsPrimary
    );

    public record CompanyEmployeeResult(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("fullName")] string FullName,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("phone")] string? Phone,
        [property: JsonPropertyName("position")] string? Position,
        [property: JsonPropertyName("hireDate")] DateTime? HireDate
    );
}
