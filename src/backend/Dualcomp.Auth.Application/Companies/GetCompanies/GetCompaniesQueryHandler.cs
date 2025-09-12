using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Domain.Domain.Common.Results;

namespace Dualcomp.Auth.Application.Companies.GetCompanies
{
    public class GetCompaniesQueryHandler : IQueryHandler<GetCompaniesQuery, GetCompaniesResult>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetCompaniesQueryHandler(ICompanyRepository companyRepository) => _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));

        public async Task<GetCompaniesResult> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _companyRepository.GetAllAsync(cancellationToken);

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                companies = companies.Where(c => 
                    c.Name.ToLowerInvariant().Contains(searchTerm) ||
                    c.TaxId.Value.ToLowerInvariant().Contains(searchTerm) ||
                    c.Addresses.Any(a => a.Address.ToLowerInvariant().Contains(searchTerm)) ||
                    c.Emails.Any(e => e.Email.Value.ToLowerInvariant().Contains(searchTerm)) ||
                    c.Phones.Any(p => p.Phone.ToLowerInvariant().Contains(searchTerm)));
            }

            var totalCount = companies.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            // Aplicar paginación
            var pagedCompanies = companies
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CompanyListItem(
                    c.Id,
                    c.Name,
                    c.TaxId.Value,
                    c.Addresses.FirstOrDefault(a => a.IsPrimary)?.Address,
                    c.Employees.Count));

            return new GetCompaniesResult(
                pagedCompanies,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages);
        }
    }
}
