using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Companies.UpdateCompany
{
    public class UpdateCompanyCommandHandler : ICommandHandler<UpdateCompanyCommand, UpdateCompanyResult>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly CompanyContactService _contactService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCompanyCommandHandler(
            ICompanyRepository companyRepository,
            CompanyContactService contactService,
            IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UpdateCompanyResult> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            // Buscar empresa con todas las colecciones
            var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
            if (company == null)
            {
                throw new InvalidOperationException("Empresa no encontrada");
            }

            // Verificar si el TaxId ya existe en otra empresa (validación eficiente)
            var taxIdExistsForOtherCompany = await _companyRepository.ExistsByTaxIdForOtherCompanyAsync(request.TaxId.Value, request.CompanyId, cancellationToken);
            if (taxIdExistsForOtherCompany)
            {
                throw new InvalidOperationException("Ya existe una empresa con este RUT");
            }

            // Validaciones de negocio - al menos un elemento de cada tipo
            if (!request.Employees?.Any() == true) throw new ArgumentException("At least one employee is required", nameof(request.Employees));
            
            // Validar contactos requeridos usando el servicio
            _contactService.ValidateRequiredContacts(request.Addresses, request.Emails, request.Phones, request.SocialMedias);

            // Actualizar información básica de la empresa
            company.UpdateInfo(request.Name, request.TaxId);

            // Limpiar colecciones existentes
            var existingAddresses = company.Addresses.ToList();
            var existingEmails = company.Emails.ToList();
            var existingPhones = company.Phones.ToList();
            var existingSocialMedias = company.SocialMedias.ToList();

            foreach (var address in existingAddresses)
            {
                company.RemoveAddress(address);
            }
            foreach (var email in existingEmails)
            {
                company.RemoveEmail(email);
            }
            foreach (var phone in existingPhones)
            {
                company.RemovePhone(phone);
            }
            foreach (var socialMedia in existingSocialMedias)
            {
                company.RemoveSocialMedia(socialMedia);
            }

            // Procesar todos los contactos usando el servicio
            var contactTypeNames = await _contactService.ProcessAllContactsAsync(
                company, 
                request.Addresses, 
                request.Emails, 
                request.Phones, 
                request.SocialMedias, 
                cancellationToken);

            // Procesar empleados (existentes y nuevos)
            foreach (var employeeDto in request.Employees)
            {
                if (employeeDto.Id.HasValue)
                {
                    // Empleado existente - actualizar información
                    var existingEmployee = company.Employees.FirstOrDefault(e => e.Id == employeeDto.Id.Value);
                    if (existingEmployee != null)
                    {
                        existingEmployee.UpdateProfile(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, employeeDto.Position);
                    }
                }
                else
                {
                    // Empleado nuevo - crear usuario automáticamente
                    var user = await _contactService.CreateUserForEmployee(employeeDto.FullName, employeeDto.Email, company.Id, cancellationToken);
                    var employee = Employee.Create(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, company.Id, employeeDto.Position, employeeDto.HireDate, user.Id);
                    company.AddEmployee(employee);
                }
            }

            // Validar que la empresa esté completa
            if (!company.IsValidForRegistration())
            {
                throw new InvalidOperationException("Company does not meet registration requirements");
            }

            await _companyRepository.UpdateAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateCompanyResult(
                company.Id,
                company.Name,
                company.TaxId.Value,
                _contactService.BuildAddressResults(company, contactTypeNames.AddressTypeNames),
                _contactService.BuildEmailResults(company, contactTypeNames.EmailTypeNames),
                _contactService.BuildPhoneResults(company, contactTypeNames.PhoneTypeNames),
                _contactService.BuildSocialMediaResults(company, contactTypeNames.SocialMediaTypeNames),
                _contactService.BuildEmployeeResults(company));
        }
    }
}
