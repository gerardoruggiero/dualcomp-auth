using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.Application.Employees.UpdateEmployee
{
    public record UpdateEmployeeCommand(
        Guid EmployeeId,
        string FullName,
        Email Email,
        string? Phone,
        string? Position
    ) : ICommand<UpdateEmployeeResult>;
}
