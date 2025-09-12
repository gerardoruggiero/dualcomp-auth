namespace Dualcomp.Auth.Application.Employees.GetEmployees
{
    public record EmployeeListItem(
        Guid Id,
        string FullName,
        string Email,
        string? Phone,
        Guid CompanyId,
        string CompanyName,
        string? Position,
        DateTime? HireDate,
        bool IsActive,
        Guid? UserId
    );

    public record GetEmployeesResult(
        IEnumerable<EmployeeListItem> Employees,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
