using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common.Results;

namespace Dualcomp.Auth.Application.Employees.CreateEmployee
{
    public class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand, CreateEmployeeResult>
    {
        private readonly ICompanyRepository _companyRepository;

        public CreateEmployeeCommandHandler(ICompanyRepository companyRepository) => _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));

        public async Task<CreateEmployeeResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Verificar que la empresa existe
            var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
            if (company == null)
            {
                throw new InvalidOperationException("Empresa no encontrada");
            }

            // Crear empleado
            var employee = Employee.Create(
                request.FullName,
                request.Email.Value,
                request.Phone,
                request.CompanyId,
                request.Position,
                request.HireDate,
                request.UserId);

            // Agregar empleado a la empresa
            company.AddEmployee(employee);
            await _companyRepository.UpdateAsync(company, cancellationToken);

            return new CreateEmployeeResult(
                employee.Id,
                employee.FullName,
                employee.Email,
                employee.Phone,
                employee.CompanyId,
                employee.Position,
                employee.HireDate,
                employee.UserId);
        }
    }
}
