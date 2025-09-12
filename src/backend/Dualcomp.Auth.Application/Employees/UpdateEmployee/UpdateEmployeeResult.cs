namespace Dualcomp.Auth.Application.Employees.UpdateEmployee
{
    public record UpdateEmployeeResult(
        Guid Id,
        string FullName,
        string Email,
        string? Phone,
        string? Position
    );
}
