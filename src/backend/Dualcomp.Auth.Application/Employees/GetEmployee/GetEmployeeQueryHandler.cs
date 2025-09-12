using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Domain.Domain.Common.Results;

namespace Dualcomp.Auth.Application.Employees.GetEmployee
{
    public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery, GetEmployeeResult>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetEmployeeQueryHandler(ICompanyRepository companyRepository) => _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));

        public async Task<GetEmployeeResult> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
        {
            // Buscar empleado en todas las empresas
            var companies = await _companyRepository.ListAsync(cancellationToken);
            var employee = companies
                .SelectMany(c => c.Employees)
                .FirstOrDefault(e => e.Id == request.EmployeeId);

            if (employee == null)
            {
                throw new InvalidOperationException("Empleado no encontrado");
            }

            // Buscar la empresa del empleado
            var company = companies.FirstOrDefault(c => c.Employees.Any(e => e.Id == request.EmployeeId));

            return new GetEmployeeResult(
                employee.Id,
                employee.FullName,
                employee.Email,
                employee.Phone,
                employee.CompanyId,
                company?.Name ?? "Empresa no encontrada",
                employee.Position,
                employee.HireDate,
                employee.IsActive,
                employee.UserId);
        }
    }
}
