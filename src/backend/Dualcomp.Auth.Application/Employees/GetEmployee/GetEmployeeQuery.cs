using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Employees.GetEmployee
{
    public record GetEmployeeQuery(
        Guid EmployeeId
    ) : IQuery<GetEmployeeResult>;
}
