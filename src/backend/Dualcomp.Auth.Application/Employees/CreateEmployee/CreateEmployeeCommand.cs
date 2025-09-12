using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.Application.Employees.CreateEmployee
{
    public record CreateEmployeeCommand(
        string FullName,
        Email Email,
        string? Phone,
        Guid CompanyId,
        string? Position = null,
        DateTime? HireDate = null,
        Guid? UserId = null
    ) : ICommand<CreateEmployeeResult>;
}
