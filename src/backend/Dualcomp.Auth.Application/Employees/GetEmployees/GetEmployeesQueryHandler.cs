using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.Application.Employees.GetEmployees
{
    public class GetEmployeesQueryHandler : IQueryHandler<GetEmployeesQuery, GetEmployeesResult>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetEmployeesQueryHandler(ICompanyRepository companyRepository) => _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));

        public async Task<GetEmployeesResult> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _companyRepository.GetAllAsync(cancellationToken);
            var allEmployees = companies
                .SelectMany(c => c.Employees.Select(e => new { Employee = e, Company = c }));

            // Filtrar por empresa si se especifica
            if (request.CompanyId.HasValue)
            {
                allEmployees = allEmployees.Where(x => x.Employee.CompanyId == request.CompanyId.Value);
            }

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                allEmployees = allEmployees.Where(x => 
                    x.Employee.FullName.ToLowerInvariant().Contains(searchTerm) ||
                    x.Employee.Email.ToLowerInvariant().Contains(searchTerm) ||
                    (x.Employee.Phone != null && x.Employee.Phone.ToLowerInvariant().Contains(searchTerm)) ||
                    (x.Employee.Position != null && x.Employee.Position.ToLowerInvariant().Contains(searchTerm)));
            }

            var totalCount = allEmployees.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            // Aplicar paginación
            var pagedEmployees = allEmployees
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new EmployeeListItem(
                    x.Employee.Id,
                    x.Employee.FullName,
                    x.Employee.Email,
                    x.Employee.Phone,
                    x.Employee.CompanyId,
                    x.Company.Name,
                    x.Employee.Position,
                    x.Employee.HireDate,
                    x.Employee.IsActive,
                    x.Employee.UserId));

            return new GetEmployeesResult(
                pagedEmployees,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages);
        }
    }
}
