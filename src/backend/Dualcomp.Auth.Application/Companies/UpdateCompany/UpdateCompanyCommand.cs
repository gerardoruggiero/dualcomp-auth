using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.Application.Companies.UpdateCompany
{
    public record UpdateCompanyCommand(
        Guid CompanyId,
        string Name,
        TaxId TaxId,
        List<UpdateCompanyAddressDto> Addresses,
        List<UpdateCompanyEmailDto> Emails,
        List<UpdateCompanyPhoneDto> Phones,
        List<UpdateCompanySocialMediaDto> SocialMedias,
        List<UpdateCompanyEmployeeDto> Employees
    ) : ICommand<UpdateCompanyResult>;

    public record UpdateCompanyAddressDto(
        Guid? Id,
        string AddressType,
        string Address,
        bool IsPrimary
    );

    public record UpdateCompanyEmailDto(
        Guid? Id,
        string EmailType,
        string Email,
        bool IsPrimary
    );

    public record UpdateCompanyPhoneDto(
        Guid? Id,
        string PhoneType,
        string Phone,
        bool IsPrimary
    );

    public record UpdateCompanySocialMediaDto(
        Guid? Id,
        string SocialMediaType,
        string Url,
        bool IsPrimary
    );

    public record UpdateCompanyEmployeeDto(
        Guid? Id,
        string FullName,
        string Email,
        string? Phone,
        string? Position,
        DateTime? HireDate
    );
}
