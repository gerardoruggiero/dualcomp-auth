using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Companies.GetCompanies
{
    public record GetCompaniesQuery(
        int Page = 1,
        int PageSize = 10,
        string? SearchTerm = null
    ) : IQuery<GetCompaniesResult>;
}
