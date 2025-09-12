namespace Dualcomp.Auth.Application.Companies.GetCompanies
{
    public record CompanyListItem(
        Guid Id,
        string Name,
        string TaxId,
        string? PrimaryAddress,
        int EmployeeCount
    );

    public record GetCompaniesResult(
        IEnumerable<CompanyListItem> Companies,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
