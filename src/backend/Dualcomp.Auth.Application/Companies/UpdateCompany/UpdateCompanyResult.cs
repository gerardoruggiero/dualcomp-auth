namespace Dualcomp.Auth.Application.Companies.UpdateCompany
{
    /// <summary>
    /// Resultado de la actualización de empresa - reutiliza GetCompanyResult para consistencia
    /// </summary>
    public record UpdateCompanyResult(
        Guid Id,
        string Name,
        string TaxId,
        List<GetCompany.CompanyAddressResult> Addresses,
        List<GetCompany.CompanyEmailResult> Emails,
        List<GetCompany.CompanyPhoneResult> Phones,
        List<GetCompany.CompanySocialMediaResult> SocialMedias,
        List<GetCompany.CompanyEmployeeResult> Employees
    );
}
