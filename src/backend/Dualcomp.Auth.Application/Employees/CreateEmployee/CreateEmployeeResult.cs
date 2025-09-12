namespace Dualcomp.Auth.Application.Employees.CreateEmployee
{
    public record CreateEmployeeResult(
        Guid Id,
        string FullName,
        string Email,
        string? Phone,
        Guid CompanyId,
        string? Position,
        DateTime? HireDate,
        Guid? UserId
    );
}
