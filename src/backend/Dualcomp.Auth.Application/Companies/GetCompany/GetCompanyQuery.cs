using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Companies.GetCompany
{
    public record GetCompanyQuery(
        Guid CompanyId
    ) : IQuery<GetCompanyResult>;
}
