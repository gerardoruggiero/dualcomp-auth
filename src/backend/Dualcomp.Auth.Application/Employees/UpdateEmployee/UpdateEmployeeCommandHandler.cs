using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common.Results;

namespace Dualcomp.Auth.Application.Employees.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, UpdateEmployeeResult>
    {
        private readonly ICompanyRepository _companyRepository;

        public UpdateEmployeeCommandHandler(ICompanyRepository companyRepository) => _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));

        public async Task<UpdateEmployeeResult> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
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

            // Actualizar información del empleado
            employee.UpdateProfile(request.FullName, request.Email.Value, request.Phone, request.Position);

            // Buscar la empresa del empleado y actualizarla
            var company = companies.FirstOrDefault(c => c.Employees.Any(e => e.Id == request.EmployeeId));
            if (company != null)
            {
                await _companyRepository.UpdateAsync(company, cancellationToken);
            }

            return new UpdateEmployeeResult(
                employee.Id,
                employee.FullName,
                employee.Email,
                employee.Phone,
                employee.Position);
        }
    }
}
