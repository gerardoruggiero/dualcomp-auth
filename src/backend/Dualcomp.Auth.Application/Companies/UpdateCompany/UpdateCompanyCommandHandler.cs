using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Companies.UpdateCompany
{
    public class UpdateCompanyCommandHandler : ICommandHandler<UpdateCompanyCommand, UpdateCompanyResult>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IAddressTypeRepository _addressTypeRepository;
        private readonly IEmailTypeRepository _emailTypeRepository;
        private readonly IPhoneTypeRepository _phoneTypeRepository;
        private readonly ISocialMediaTypeRepository _socialMediaTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCompanyCommandHandler(
            ICompanyRepository companyRepository,
            IAddressTypeRepository addressTypeRepository,
            IEmailTypeRepository emailTypeRepository,
            IPhoneTypeRepository phoneTypeRepository,
            ISocialMediaTypeRepository socialMediaTypeRepository,
            IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _addressTypeRepository = addressTypeRepository ?? throw new ArgumentNullException(nameof(addressTypeRepository));
            _emailTypeRepository = emailTypeRepository ?? throw new ArgumentNullException(nameof(emailTypeRepository));
            _phoneTypeRepository = phoneTypeRepository ?? throw new ArgumentNullException(nameof(phoneTypeRepository));
            _socialMediaTypeRepository = socialMediaTypeRepository ?? throw new ArgumentNullException(nameof(socialMediaTypeRepository));
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

            // Verificar si el TaxId ya existe en otra empresa
            var existingCompany = await _companyRepository.GetByTaxIdAsync(request.TaxId, cancellationToken);
            if (existingCompany != null && existingCompany.Id != request.CompanyId)
            {
                throw new InvalidOperationException("Ya existe una empresa con este RUT");
            }

            // Validaciones de negocio - al menos un elemento de cada tipo
            if (!request.Addresses?.Any() == true) throw new ArgumentException("At least one address is required", nameof(request.Addresses));
            if (!request.Emails?.Any() == true) throw new ArgumentException("At least one email is required", nameof(request.Emails));
            if (!request.Phones?.Any() == true) throw new ArgumentException("At least one phone is required", nameof(request.Phones));
            if (!request.SocialMedias?.Any() == true) throw new ArgumentException("At least one social media is required", nameof(request.SocialMedias));
            if (!request.Employees?.Any() == true) throw new ArgumentException("At least one employee is required", nameof(request.Employees));

            // Actualizar información básica de la empresa
            company.UpdateInfo(request.Name, request.TaxId);

            // Limpiar colecciones existentes
            var existingAddresses = company.Addresses.ToList();
            var existingEmails = company.Emails.ToList();
            var existingPhones = company.Phones.ToList();
            var existingSocialMedias = company.SocialMedias.ToList();
            var existingEmployees = company.Employees.ToList();

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

            // Obtener todos los tipos de contacto
            var addressTypes = await _addressTypeRepository.GetAllAsync(cancellationToken);
            var emailTypes = await _emailTypeRepository.GetAllAsync(cancellationToken);
            var phoneTypes = await _phoneTypeRepository.GetAllAsync(cancellationToken);
            var socialMediaTypes = await _socialMediaTypeRepository.GetAllAsync(cancellationToken);

            // Crear diccionarios para mapear IDs a nombres
            var addressTypeNames = addressTypes.ToDictionary(at => at.Id, at => at.Name);
            var emailTypeNames = emailTypes.ToDictionary(et => et.Id, et => et.Name);
            var phoneTypeNames = phoneTypes.ToDictionary(pt => pt.Id, pt => pt.Name);
            var socialMediaTypeNames = socialMediaTypes.ToDictionary(smt => smt.Id, smt => smt.Name);

            // Agregar nuevas direcciones
            foreach (var addressDto in request.Addresses)
            {
                var addressTypeEntity = addressTypes.FirstOrDefault(at => at.Name == addressDto.AddressType);
                if (addressTypeEntity == null)
                    throw new ArgumentException($"Address type '{addressDto.AddressType}' not found");
                
                var address = CompanyAddress.Create(company.Id, addressTypeEntity.Id, addressDto.Address, addressDto.IsPrimary);
                company.AddAddress(address);
            }

            // Agregar nuevos emails
            foreach (var emailDto in request.Emails)
            {
                var emailTypeEntity = emailTypes.FirstOrDefault(et => et.Name == emailDto.EmailType);
                if (emailTypeEntity == null)
                    throw new ArgumentException($"Email type '{emailDto.EmailType}' not found");
                
                var email = Email.Create(emailDto.Email);
                var companyEmail = CompanyEmail.Create(company.Id, emailTypeEntity.Id, email, emailDto.IsPrimary);
                company.AddEmail(companyEmail);
            }

            // Agregar nuevos teléfonos
            foreach (var phoneDto in request.Phones)
            {
                var phoneTypeEntity = phoneTypes.FirstOrDefault(pt => pt.Name == phoneDto.PhoneType);
                if (phoneTypeEntity == null)
                    throw new ArgumentException($"Phone type '{phoneDto.PhoneType}' not found");
                
                var phone = CompanyPhone.Create(company.Id, phoneTypeEntity.Id, phoneDto.Phone, phoneDto.IsPrimary);
                company.AddPhone(phone);
            }

            // Agregar nuevas redes sociales
            foreach (var socialMediaDto in request.SocialMedias)
            {
                var socialMediaTypeEntity = socialMediaTypes.FirstOrDefault(smt => smt.Name == socialMediaDto.SocialMediaType);
                if (socialMediaTypeEntity == null)
                    throw new ArgumentException($"Social media type '{socialMediaDto.SocialMediaType}' not found");
                
                var socialMedia = CompanySocialMedia.Create(company.Id, socialMediaTypeEntity.Id, socialMediaDto.Url, socialMediaDto.IsPrimary);
                company.AddSocialMedia(socialMedia);
            }

            // Agregar nuevos empleados
            foreach (var employeeDto in request.Employees)
            {
                var employee = Employee.Create(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, company.Id, employeeDto.Position, employeeDto.HireDate);
                company.AddEmployee(employee);
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
                company.Addresses.Select(a => new CompanyAddressResult(addressTypeNames[a.AddressTypeId], a.Address, a.IsPrimary)).ToList(),
                company.Emails.Select(e => new CompanyEmailResult(emailTypeNames[e.EmailTypeId], e.Email.Value, e.IsPrimary)).ToList(),
                company.Phones.Select(p => new CompanyPhoneResult(phoneTypeNames[p.PhoneTypeId], p.Phone, p.IsPrimary)).ToList(),
                company.SocialMedias.Select(sm => new CompanySocialMediaResult(socialMediaTypeNames[sm.SocialMediaTypeId], sm.Url, sm.IsPrimary)).ToList(),
                company.Employees.Select(e => new CompanyEmployeeResult(e.FullName, e.Email, e.Phone, e.Position, e.HireDate)).ToList());
        }
    }
}
