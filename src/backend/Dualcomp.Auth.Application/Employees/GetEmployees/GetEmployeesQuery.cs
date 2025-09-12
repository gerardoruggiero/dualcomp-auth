using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Employees.GetEmployees
{
    public record GetEmployeesQuery(
        Guid? CompanyId = null,
        int Page = 1,
        int PageSize = 10,
        string? SearchTerm = null
    ) : IQuery<GetEmployeesResult>;
}
