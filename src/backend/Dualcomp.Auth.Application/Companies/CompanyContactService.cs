using Dualcomp.Auth.Application.Companies.GetCompany;
using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Application.Companies.UpdateCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Security;

namespace Dualcomp.Auth.Application.Companies
{
    public class CompanyContactService : ICompanyContactService
    {
        private readonly IAddressTypeRepository _addressTypeRepository;
        private readonly IEmailTypeRepository _emailTypeRepository;
        private readonly IPhoneTypeRepository _phoneTypeRepository;
        private readonly ISocialMediaTypeRepository _socialMediaTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordGenerator _passwordGenerator;

        public CompanyContactService(
            IAddressTypeRepository addressTypeRepository,
            IEmailTypeRepository emailTypeRepository,
            IPhoneTypeRepository phoneTypeRepository,
            ISocialMediaTypeRepository socialMediaTypeRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IPasswordGenerator passwordGenerator)
        {
            _addressTypeRepository = addressTypeRepository ?? throw new ArgumentNullException(nameof(addressTypeRepository));
            _emailTypeRepository = emailTypeRepository ?? throw new ArgumentNullException(nameof(emailTypeRepository));
            _phoneTypeRepository = phoneTypeRepository ?? throw new ArgumentNullException(nameof(phoneTypeRepository));
            _socialMediaTypeRepository = socialMediaTypeRepository ?? throw new ArgumentNullException(nameof(socialMediaTypeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
        }

        /// <summary>
        /// Valida que se proporcionen al menos un elemento de cada tipo de contacto requerido para registro
        /// </summary>
        public void ValidateRequiredContactsForRegistration(
            IEnumerable<RegisterCompanyAddressDto>? addresses, 
            IEnumerable<RegisterCompanyEmailDto>? emails, 
            IEnumerable<RegisterCompanyPhoneDto>? phones, 
            IEnumerable<RegisterCompanySocialMediaDto>? socialMedias)
        {
            if (!addresses?.Any() == true) throw new ArgumentException("At least one address is required", nameof(addresses));
            if (!emails?.Any() == true) throw new ArgumentException("At least one email is required", nameof(emails));
            if (!phones?.Any() == true) throw new ArgumentException("At least one phone is required", nameof(phones));
            if (!socialMedias?.Any() == true) throw new ArgumentException("At least one social media is required", nameof(socialMedias));
        }

        /// <summary>
        /// Valida que se proporcionen al menos un elemento de cada tipo de contacto requerido para actualización
        /// </summary>
        public void ValidateRequiredContactsForUpdate(
            IEnumerable<UpdateCompanyAddressDto>? addresses, 
            IEnumerable<UpdateCompanyEmailDto>? emails, 
            IEnumerable<UpdateCompanyPhoneDto>? phones, 
            IEnumerable<UpdateCompanySocialMediaDto>? socialMedias)
        {
            if (!addresses?.Any() == true) throw new ArgumentException("At least one address is required", nameof(addresses));
            if (!emails?.Any() == true) throw new ArgumentException("At least one email is required", nameof(emails));
            if (!phones?.Any() == true) throw new ArgumentException("At least one phone is required", nameof(phones));
            if (!socialMedias?.Any() == true) throw new ArgumentException("At least one social media is required", nameof(socialMedias));
        }

        /// <summary>
        /// Procesa direcciones para registro (solo nuevas direcciones)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessAddressesForRegistrationAsync(Company company, IEnumerable<RegisterCompanyAddressDto> addressDtos, CancellationToken cancellationToken)
        {
            var addressTypeNames = new Dictionary<Guid, string>();

            foreach (var addressDto in addressDtos)
            {
                var addressTypeEntity = await _addressTypeRepository.GetByIdAsync(addressDto.AddressTypeId, cancellationToken);
                if (addressTypeEntity == null)
                    throw new ArgumentException($"Address type with ID '{addressDto.AddressTypeId}' not found");

                // Agregar al diccionario para el resultado
                addressTypeNames[addressTypeEntity.Id] = addressTypeEntity.Name;

                // Crear nueva dirección (en registro siempre son nuevas)
                var address = CompanyAddress.Create(company.Id, addressTypeEntity.Id, addressDto.Address, addressDto.IsPrimary);
                company.AddAddress(address);
            }

            return addressTypeNames;
        }

        /// <summary>
        /// Procesa direcciones para actualización (pueden ser nuevas o existentes)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessAddressesForUpdateAsync(Company company, IEnumerable<UpdateCompanyAddressDto> addressDtos, CancellationToken cancellationToken)
        {
            var addressTypeNames = new Dictionary<Guid, string>();

            foreach (var addressDto in addressDtos)
            {
                var addressTypeEntity = await _addressTypeRepository.GetByIdAsync(addressDto.AddressTypeId, cancellationToken);
                if (addressTypeEntity == null)
                    throw new ArgumentException($"Address type with ID '{addressDto.AddressTypeId}' not found");

                // Agregar al diccionario para el resultado
                addressTypeNames[addressTypeEntity.Id] = addressTypeEntity.Name;

                // Verificar si es una dirección existente o nueva
                if (addressDto.Id.HasValue && addressDto.Id.Value != Guid.Empty)
                {
                    // Dirección existente - buscar y actualizar
                    var existingAddress = company.Addresses.FirstOrDefault(a => a.Id == addressDto.Id.Value);
                    if (existingAddress != null)
                    {
                        existingAddress.UpdateInfo(addressTypeEntity.Id, addressDto.Address, addressDto.IsPrimary);
                    }
                }
                else
                {
                    // Dirección nueva - crear
                    var address = CompanyAddress.Create(company.Id, addressTypeEntity.Id, addressDto.Address, addressDto.IsPrimary);
                    company.AddAddress(address);
                }
            }

            return addressTypeNames;
        }

        /// <summary>
        /// Procesa emails para registro (solo nuevos emails)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessEmailsForRegistrationAsync(Company company, IEnumerable<RegisterCompanyEmailDto> emailDtos, CancellationToken cancellationToken)
        {
            var emailTypeNames = new Dictionary<Guid, string>();

            foreach (var emailDto in emailDtos)
            {
                var emailTypeEntity = await _emailTypeRepository.GetByIdAsync(emailDto.EmailTypeId, cancellationToken);
                if (emailTypeEntity == null)
                    throw new ArgumentException($"Email type with ID '{emailDto.EmailTypeId}' not found");

                // Agregar al diccionario para el resultado
                emailTypeNames[emailTypeEntity.Id] = emailTypeEntity.Name;

                // Crear nuevo email (en registro siempre son nuevos)
                var email = Email.Create(emailDto.Email);
                var companyEmail = CompanyEmail.Create(company.Id, emailTypeEntity.Id, email, emailDto.IsPrimary);
                company.AddEmail(companyEmail);
            }

            return emailTypeNames;
        }

        /// <summary>
        /// Procesa emails para actualización (pueden ser nuevos o existentes)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessEmailsForUpdateAsync(Company company, IEnumerable<UpdateCompanyEmailDto> emailDtos, CancellationToken cancellationToken)
        {
            var emailTypeNames = new Dictionary<Guid, string>();

            foreach (var emailDto in emailDtos)
            {
                var emailTypeEntity = await _emailTypeRepository.GetByIdAsync(emailDto.EmailTypeId, cancellationToken);
                if (emailTypeEntity == null)
                    throw new ArgumentException($"Email type with ID '{emailDto.EmailTypeId}' not found");

                // Agregar al diccionario para el resultado
                emailTypeNames[emailTypeEntity.Id] = emailTypeEntity.Name;

                // Verificar si es un email existente o nuevo
                if (emailDto.Id.HasValue && emailDto.Id.Value != Guid.Empty)
                {
                    // Email existente - buscar y actualizar
                    var existingEmail = company.Emails.FirstOrDefault(e => e.Id == emailDto.Id.Value);
                    if (existingEmail != null)
                    {
                        var email = Email.Create(emailDto.Email);
                        existingEmail.UpdateInfo(emailTypeEntity.Id, email, emailDto.IsPrimary);
                    }
                }
                else
                {
                    // Email nuevo - crear
                    var email = Email.Create(emailDto.Email);
                    var companyEmail = CompanyEmail.Create(company.Id, emailTypeEntity.Id, email, emailDto.IsPrimary);
                    company.AddEmail(companyEmail);
                }
            }

            return emailTypeNames;
        }

        /// <summary>
        /// Procesa teléfonos para registro (solo nuevos teléfonos)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessPhonesForRegistrationAsync(Company company, IEnumerable<RegisterCompanyPhoneDto> phoneDtos, CancellationToken cancellationToken)
        {
            var phoneTypeNames = new Dictionary<Guid, string>();

            foreach (var phoneDto in phoneDtos)
            {
                var phoneTypeEntity = await _phoneTypeRepository.GetByIdAsync(phoneDto.PhoneTypeId, cancellationToken);
                if (phoneTypeEntity == null)
                    throw new ArgumentException($"Phone type with ID '{phoneDto.PhoneTypeId}' not found");

                // Agregar al diccionario para el resultado
                phoneTypeNames[phoneTypeEntity.Id] = phoneTypeEntity.Name;

                // Crear nuevo teléfono (en registro siempre son nuevos)
                var phone = CompanyPhone.Create(company.Id, phoneTypeEntity.Id, phoneDto.Phone, phoneDto.IsPrimary);
                company.AddPhone(phone);
            }

            return phoneTypeNames;
        }

        /// <summary>
        /// Procesa teléfonos para actualización (pueden ser nuevos o existentes)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessPhonesForUpdateAsync(Company company, IEnumerable<UpdateCompanyPhoneDto> phoneDtos, CancellationToken cancellationToken)
        {
            var phoneTypeNames = new Dictionary<Guid, string>();

            foreach (var phoneDto in phoneDtos)
            {
                var phoneTypeEntity = await _phoneTypeRepository.GetByIdAsync(phoneDto.PhoneTypeId, cancellationToken);
                if (phoneTypeEntity == null)
                    throw new ArgumentException($"Phone type with ID '{phoneDto.PhoneTypeId}' not found");

                // Agregar al diccionario para el resultado
                phoneTypeNames[phoneTypeEntity.Id] = phoneTypeEntity.Name;

                // Verificar si es un teléfono existente o nuevo
                if (phoneDto.Id.HasValue && phoneDto.Id.Value != Guid.Empty)
                {
                    // Teléfono existente - buscar y actualizar
                    var existingPhone = company.Phones.FirstOrDefault(p => p.Id == phoneDto.Id.Value);
                    if (existingPhone != null)
                    {
                        existingPhone.UpdateInfo(phoneTypeEntity.Id, phoneDto.Phone, phoneDto.IsPrimary);
                    }
                }
                else
                {
                    // Teléfono nuevo - crear
                    var phone = CompanyPhone.Create(company.Id, phoneTypeEntity.Id, phoneDto.Phone, phoneDto.IsPrimary);
                    company.AddPhone(phone);
                }
            }

            return phoneTypeNames;
        }

        /// <summary>
        /// Procesa redes sociales para registro (solo nuevas redes sociales)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessSocialMediasForRegistrationAsync(Company company, IEnumerable<RegisterCompanySocialMediaDto> socialMediaDtos, CancellationToken cancellationToken)
        {
            var socialMediaTypeNames = new Dictionary<Guid, string>();

            foreach (var socialMediaDto in socialMediaDtos)
            {
                var socialMediaTypeEntity = await _socialMediaTypeRepository.GetByIdAsync(socialMediaDto.SocialMediaTypeId, cancellationToken);
                if (socialMediaTypeEntity == null)
                    throw new ArgumentException($"Social media type with ID '{socialMediaDto.SocialMediaTypeId}' not found");

                // Agregar al diccionario para el resultado
                socialMediaTypeNames[socialMediaTypeEntity.Id] = socialMediaTypeEntity.Name;

                // Crear nueva red social (en registro siempre son nuevas)
                var socialMedia = CompanySocialMedia.Create(company.Id, socialMediaTypeEntity.Id, socialMediaDto.Url, socialMediaDto.IsPrimary);
                company.AddSocialMedia(socialMedia);
            }

            return socialMediaTypeNames;
        }

        /// <summary>
        /// Procesa redes sociales para actualización (pueden ser nuevas o existentes)
        /// </summary>
        private async Task<Dictionary<Guid, string>> ProcessSocialMediasForUpdateAsync(Company company, IEnumerable<UpdateCompanySocialMediaDto> socialMediaDtos, CancellationToken cancellationToken)
        {
            var socialMediaTypeNames = new Dictionary<Guid, string>();

            foreach (var socialMediaDto in socialMediaDtos)
            {
                var socialMediaTypeEntity = await _socialMediaTypeRepository.GetByIdAsync(socialMediaDto.SocialMediaTypeId, cancellationToken);
                if (socialMediaTypeEntity == null)
                    throw new ArgumentException($"Social media type with ID '{socialMediaDto.SocialMediaTypeId}' not found");

                // Agregar al diccionario para el resultado
                socialMediaTypeNames[socialMediaTypeEntity.Id] = socialMediaTypeEntity.Name;

                // Verificar si es una red social existente o nueva
                if (socialMediaDto.Id.HasValue && socialMediaDto.Id.Value != Guid.Empty)
                {
                    // Red social existente - buscar y actualizar
                    var existingSocialMedia = company.SocialMedias.FirstOrDefault(sm => sm.Id == socialMediaDto.Id.Value);
                    if (existingSocialMedia != null)
                    {
                        existingSocialMedia.UpdateInfo(socialMediaTypeEntity.Id, socialMediaDto.Url, socialMediaDto.IsPrimary);
                    }
                }
                else
                {
                    // Red social nueva - crear
                    var socialMedia = CompanySocialMedia.Create(company.Id, socialMediaTypeEntity.Id, socialMediaDto.Url, socialMediaDto.IsPrimary);
                    company.AddSocialMedia(socialMedia);
                }
            }

            return socialMediaTypeNames;
        }

        /// <summary>
        /// Procesa todos los tipos de contacto de una vez para registro
        /// </summary>
        public async Task<ContactTypeNames> ProcessAllContactsForRegistrationAsync(
            Company company, 
            IEnumerable<RegisterCompanyAddressDto> addresses, 
            IEnumerable<RegisterCompanyEmailDto> emails, 
            IEnumerable<RegisterCompanyPhoneDto> phones, 
            IEnumerable<RegisterCompanySocialMediaDto> socialMedias, 
            CancellationToken cancellationToken)
        {
            // Procesar contactos secuencialmente para evitar problemas de concurrencia con DbContext
            var addressTypeNames = await ProcessAddressesForRegistrationAsync(company, addresses, cancellationToken);
            var emailTypeNames = await ProcessEmailsForRegistrationAsync(company, emails, cancellationToken);
            var phoneTypeNames = await ProcessPhonesForRegistrationAsync(company, phones, cancellationToken);
            var socialMediaTypeNames = await ProcessSocialMediasForRegistrationAsync(company, socialMedias, cancellationToken);

            return new ContactTypeNames(
                addressTypeNames,
                emailTypeNames,
                phoneTypeNames,
                socialMediaTypeNames
            );
        }

        /// <summary>
        /// Procesa todos los tipos de contacto de una vez para actualización
        /// </summary>
        public async Task<ContactTypeNames> ProcessAllContactsForUpdateAsync(
            Company company, 
            IEnumerable<UpdateCompanyAddressDto> addresses, 
            IEnumerable<UpdateCompanyEmailDto> emails, 
            IEnumerable<UpdateCompanyPhoneDto> phones, 
            IEnumerable<UpdateCompanySocialMediaDto> socialMedias, 
            CancellationToken cancellationToken)
        {
            // Procesar contactos secuencialmente para evitar problemas de concurrencia con DbContext
            var addressTypeNames = await ProcessAddressesForUpdateAsync(company, addresses, cancellationToken);
            var emailTypeNames = await ProcessEmailsForUpdateAsync(company, emails, cancellationToken);
            var phoneTypeNames = await ProcessPhonesForUpdateAsync(company, phones, cancellationToken);
            var socialMediaTypeNames = await ProcessSocialMediasForUpdateAsync(company, socialMedias, cancellationToken);

            return new ContactTypeNames(
                addressTypeNames,
                emailTypeNames,
                phoneTypeNames,
                socialMediaTypeNames
            );
        }

        /// <summary>
        /// Elimina contactos que ya no están en la request (fueron eliminados por el usuario)
        /// </summary>
        public async Task RemoveDeletedContactsAsync(
            Company company, 
            IEnumerable<UpdateCompanyAddressDto> addresses, 
            IEnumerable<UpdateCompanyEmailDto> emails, 
            IEnumerable<UpdateCompanyPhoneDto> phones, 
            IEnumerable<UpdateCompanySocialMediaDto> socialMedias, 
            CancellationToken cancellationToken)
        {
            // Obtener IDs de contactos que están en la request
            var requestedAddressIds = addresses
                .Where(a => a.Id.HasValue && a.Id.Value != Guid.Empty)
                .Select(a => a.Id.Value)
                .ToHashSet();

            var requestedEmailIds = emails
                .Where(e => e.Id.HasValue && e.Id.Value != Guid.Empty)
                .Select(e => e.Id.Value)
                .ToHashSet();

            var requestedPhoneIds = phones
                .Where(p => p.Id.HasValue && p.Id.Value != Guid.Empty)
                .Select(p => p.Id.Value)
                .ToHashSet();

            var requestedSocialMediaIds = socialMedias
                .Where(sm => sm.Id.HasValue && sm.Id.Value != Guid.Empty)
                .Select(sm => sm.Id.Value)
                .ToHashSet();

            // Eliminar direcciones que no están en la request
            var addressesToRemove = company.Addresses
                .Where(a => !requestedAddressIds.Contains(a.Id))
                .ToList();
            foreach (var address in addressesToRemove)
            {
                company.RemoveAddress(address);
            }

            // Eliminar emails que no están en la request
            var emailsToRemove = company.Emails
                .Where(e => !requestedEmailIds.Contains(e.Id))
                .ToList();
            foreach (var email in emailsToRemove)
            {
                company.RemoveEmail(email);
            }

            // Eliminar teléfonos que no están en la request
            var phonesToRemove = company.Phones
                .Where(p => !requestedPhoneIds.Contains(p.Id))
                .ToList();
            foreach (var phone in phonesToRemove)
            {
                company.RemovePhone(phone);
            }

            // Eliminar redes sociales que no están en la request
            var socialMediasToRemove = company.SocialMedias
                .Where(sm => !requestedSocialMediaIds.Contains(sm.Id))
                .ToList();
            foreach (var socialMedia in socialMediasToRemove)
            {
                company.RemoveSocialMedia(socialMedia);
            }

            await Task.CompletedTask; // Para mantener la signatura async
        }

        /// <summary>
        /// Construye los resultados de direcciones
        /// </summary>
        public List<CompanyAddressResult> BuildAddressResults(Company company, Dictionary<Guid, string> addressTypeNames)
        {
            return company.Addresses.Select(a => new CompanyAddressResult(a.Id.ToString(), a.AddressTypeId.ToString(), a.Address, a.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de emails
        /// </summary>
        public List<CompanyEmailResult> BuildEmailResults(Company company, Dictionary<Guid, string> emailTypeNames)
        {
            return company.Emails.Select(e => new CompanyEmailResult(e.Id.ToString(), e.EmailTypeId.ToString(), e.Email.Value, e.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de teléfonos
        /// </summary>
        public List<CompanyPhoneResult> BuildPhoneResults(Company company, Dictionary<Guid, string> phoneTypeNames)
        {
            return company.Phones.Select(p => new CompanyPhoneResult(p.Id.ToString(), p.PhoneTypeId.ToString(), p.Phone, p.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de redes sociales
        /// </summary>
        public List<CompanySocialMediaResult> BuildSocialMediaResults(Company company, Dictionary<Guid, string> socialMediaTypeNames)
        {
            return company.SocialMedias.Select(sm => new CompanySocialMediaResult(sm.Id.ToString(), sm.SocialMediaTypeId.ToString(), sm.Url, sm.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de empleados (solo empleados activos)
        /// </summary>
        public List<CompanyEmployeeResult> BuildEmployeeResults(Company company)
        {
            return company.Employees
                .Where(e => e.IsActive)
                .Select(e => new CompanyEmployeeResult(e.Id.ToString(), e.FullName, e.Email, e.Phone, e.Position, e.HireDate))
                .ToList();
        }

        /// <summary>
        /// Construye todos los resultados de contacto de la empresa de manera optimizada
        /// </summary>
        public async Task<CompanyContactResults> BuildAllContactResultsAsync(Company company, CancellationToken cancellationToken)
        {
            // Obtener todos los tipos únicos necesarios
            var addressTypeIds = company.Addresses.Select(a => a.AddressTypeId).Distinct().ToList();
            var emailTypeIds = company.Emails.Select(e => e.EmailTypeId).Distinct().ToList();
            var phoneTypeIds = company.Phones.Select(p => p.PhoneTypeId).Distinct().ToList();
            var socialMediaTypeIds = company.SocialMedias.Select(sm => sm.SocialMediaTypeId).Distinct().ToList();

            // Obtener tipos secuencialmente para evitar problemas de concurrencia con DbContext
            var addressTypes = await GetAddressTypesByIdsAsync(addressTypeIds, cancellationToken);
            var emailTypes = await GetEmailTypesByIdsAsync(emailTypeIds, cancellationToken);
            var phoneTypes = await GetPhoneTypesByIdsAsync(phoneTypeIds, cancellationToken);
            var socialMediaTypes = await GetSocialMediaTypesByIdsAsync(socialMediaTypeIds, cancellationToken);

            // Construir los resultados
            var addressResults = company.Addresses.Select(a => 
                new CompanyAddressResult(a.Id.ToString(), a.AddressTypeId.ToString(), a.Address, a.IsPrimary)).ToList();
            
            var emailResults = company.Emails.Select(e => 
                new CompanyEmailResult(e.Id.ToString(), e.EmailTypeId.ToString(), e.Email.Value, e.IsPrimary)).ToList();
            
            var phoneResults = company.Phones.Select(p => 
                new CompanyPhoneResult(p.Id.ToString(), p.PhoneTypeId.ToString(), p.Phone, p.IsPrimary)).ToList();
            
            var socialMediaResults = company.SocialMedias.Select(sm => 
                new CompanySocialMediaResult(sm.Id.ToString(), sm.SocialMediaTypeId.ToString(), sm.Url, sm.IsPrimary)).ToList();
            
            var employeeResults = company.Employees
                .Where(e => e.IsActive)
                .Select(e => new CompanyEmployeeResult(e.Id.ToString(), e.FullName, e.Email, e.Phone, e.Position, e.HireDate))
                .ToList();

            return new CompanyContactResults(addressResults, emailResults, phoneResults, socialMediaResults, employeeResults);
        }

        private async Task<Dictionary<Guid, string>> GetAddressTypesByIdsAsync(List<Guid> typeIds, CancellationToken cancellationToken)
        {
            var result = new Dictionary<Guid, string>();
            foreach (var typeId in typeIds)
            {
                var type = await _addressTypeRepository.GetByIdAsync(typeId, cancellationToken);
                if (type != null)
                    result[typeId] = type.Name;
            }
            return result;
        }

        private async Task<Dictionary<Guid, string>> GetEmailTypesByIdsAsync(List<Guid> typeIds, CancellationToken cancellationToken)
        {
            var result = new Dictionary<Guid, string>();
            foreach (var typeId in typeIds)
            {
                var type = await _emailTypeRepository.GetByIdAsync(typeId, cancellationToken);
                if (type != null)
                    result[typeId] = type.Name;
            }
            return result;
        }

        private async Task<Dictionary<Guid, string>> GetPhoneTypesByIdsAsync(List<Guid> typeIds, CancellationToken cancellationToken)
        {
            var result = new Dictionary<Guid, string>();
            foreach (var typeId in typeIds)
            {
                var type = await _phoneTypeRepository.GetByIdAsync(typeId, cancellationToken);
                if (type != null)
                    result[typeId] = type.Name;
            }
            return result;
        }

        private async Task<Dictionary<Guid, string>> GetSocialMediaTypesByIdsAsync(List<Guid> typeIds, CancellationToken cancellationToken)
        {
            var result = new Dictionary<Guid, string>();
            foreach (var typeId in typeIds)
            {
                var type = await _socialMediaTypeRepository.GetByIdAsync(typeId, cancellationToken);
                if (type != null)
                    result[typeId] = type.Name;
            }
            return result;
        }

        /// <summary>
        /// Crea un usuario automáticamente para un empleado
        /// </summary>
        public async Task<User> CreateUserForEmployee(string fullName, string email, Guid companyId, CancellationToken cancellationToken)
        {
            // Crear el email y verificar que no exista ya un usuario con este email
            var emailValueObject = Email.Create(email);
            var existingUser = await _userRepository.GetByEmailAsync(emailValueObject, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"A user with email '{email}' already exists");
            }

            // Parsear el nombre completo
            var nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.Length > 0 ? nameParts[0] : fullName;
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "User"; // Default lastName if only one name provided

            // Generar contraseña temporal que cumpla con las políticas
            var temporaryPassword = _passwordGenerator.GenerateTemporaryPassword();
            var hashedPassword = HashedPassword.Create(_passwordHasher.HashPassword(temporaryPassword));

            // Crear el usuario
            var user = User.Create(firstName, lastName, emailValueObject, hashedPassword, companyId, false);
            
            // Guardar el usuario
            await _userRepository.AddAsync(user, cancellationToken);

            return user;
            // TODO: Enviar la contraseña temporal por email al empleado
            // Por ahora, la contraseña se genera pero no se comunica al usuario
            // En una implementación completa, se debería enviar por email o SMS
        }

        /// <summary>
        /// Procesa empleados para actualización (pueden ser nuevos o existentes)
        /// </summary>
        public async Task ProcessEmployeesForUpdateAsync(Company company, IEnumerable<UpdateCompanyEmployeeDto> employees, CancellationToken cancellationToken)
        {
            foreach (var employeeDto in employees)
            {
                if (employeeDto.Id.HasValue && employeeDto.Id.Value != Guid.Empty)
                {
                    // Empleado existente - buscar y actualizar
                    var existingEmployee = company.Employees.FirstOrDefault(e => e.Id == employeeDto.Id.Value);
                    if (existingEmployee != null)
                    {
                        existingEmployee.UpdateProfile(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, employeeDto.Position);
                    }
                }
                else
                {
                    // Empleado nuevo - crear usuario primero (ya se guarda en CreateUserForEmployee), y luego crear empleado
                    var user = await CreateUserForEmployee(employeeDto.FullName, employeeDto.Email, company.Id, cancellationToken);

                    var userEntity = await _userRepository.GetByIdAsync(user.Id, cancellationToken);

                    // Crear el empleado con el UserId del usuario guardado
                    var employee = Employee.Create(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, company.Id, employeeDto.Position, employeeDto.HireDate, user);
                    company.AddEmployee(employee);
                }
            }
        }

        /// <summary>
        /// Desactiva empleados que ya no están en la request (fueron eliminados por el usuario)
        /// </summary>
        public async Task DeactivateDeletedEmployeesAsync(
            Company company, 
            IEnumerable<UpdateCompanyEmployeeDto> employees, 
            CancellationToken cancellationToken)
        {
            // Obtener IDs de empleados que están en la request
            var requestedEmployeeIds = employees
                .Where(e => e.Id.HasValue && e.Id.Value != Guid.Empty)
                .Select(e => e.Id.Value)
                .ToHashSet();

            // Desactivar empleados que no están en la request
            var employeesToDeactivate = company.Employees
                .Where(e => !requestedEmployeeIds.Contains(e.Id) && e.IsActive)
                .ToList();

            foreach (var employee in employeesToDeactivate)
            {
                // Desactivar el empleado
                employee.Deactivate();
                
                // Si el empleado tiene un usuario asociado, también desactivarlo
                if (employee.UserId.HasValue)
                {
                    var user = await _userRepository.GetByIdAsync(employee.UserId.Value, cancellationToken);
                    if (user != null)
                    {
                        user.Deactivate();
                    }
                }
            }

            await Task.CompletedTask; // Para mantener la signatura async
        }

        /// <summary>
        /// Procesa empleados para registro (solo nuevos empleados)
        /// </summary>
        public async Task ProcessEmployeesForRegistrationAsync(Company company, IEnumerable<RegisterCompanyEmployeeDto> employees, CancellationToken cancellationToken)
        {
            foreach (var employeeDto in employees)
            {
                // En registro siempre son empleados nuevos - crear usuario primero (ya se guarda en CreateUserForEmployee), y luego crear empleado
                var user = await CreateUserForEmployee(employeeDto.FullName, employeeDto.Email, company.Id, cancellationToken);
                
                // Crear el empleado con el UserId del usuario guardado
                var employee = Employee.Create(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, company.Id, employeeDto.Position, employeeDto.HireDate, user.Id);
                company.AddEmployee(employee);
            }
        }
    }

    /// <summary>
    /// Contenedor para los diccionarios de nombres de tipos de contacto
    /// </summary>
    public record ContactTypeNames(
        Dictionary<Guid, string> AddressTypeNames,
        Dictionary<Guid, string> EmailTypeNames,
        Dictionary<Guid, string> PhoneTypeNames,
        Dictionary<Guid, string> SocialMediaTypeNames
    );

    /// <summary>
    /// Contenedor para todos los resultados de contacto de una empresa
    /// </summary>
    public record CompanyContactResults(
        List<CompanyAddressResult> Addresses,
        List<CompanyEmailResult> Emails,
        List<CompanyPhoneResult> Phones,
        List<CompanySocialMediaResult> SocialMedias,
        List<CompanyEmployeeResult> Employees
    );
}
