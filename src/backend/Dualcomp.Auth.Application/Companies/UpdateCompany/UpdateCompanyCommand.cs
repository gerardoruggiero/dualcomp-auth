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
        List<UpdateCompanyEmployeeDto> Employees,
        List<Guid> ModuleIds
    ) : ICommand<UpdateCompanyResult>;

    public record UpdateCompanyAddressDto(
        Guid? Id,
        Guid AddressTypeId,
        string Address,
        bool IsPrimary
    );

    public record UpdateCompanyEmailDto(
        Guid? Id,
        Guid EmailTypeId,
        string Email,
        bool IsPrimary
    );

    public record UpdateCompanyPhoneDto(
        Guid? Id,
        Guid PhoneTypeId,
        string Phone,
        bool IsPrimary
    );

    public record UpdateCompanySocialMediaDto(
        Guid? Id,
        Guid SocialMediaTypeId,
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
