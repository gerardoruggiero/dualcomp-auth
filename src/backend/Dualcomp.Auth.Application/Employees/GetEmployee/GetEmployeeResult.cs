namespace Dualcomp.Auth.Application.Employees.GetEmployee
{
    public record GetEmployeeResult(
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
}
