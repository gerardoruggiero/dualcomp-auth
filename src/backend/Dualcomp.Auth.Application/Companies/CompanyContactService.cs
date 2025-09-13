using Dualcomp.Auth.Application.Companies.GetCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
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
        /// Valida que se proporcionen al menos un elemento de cada tipo de contacto requerido
        /// </summary>
        public void ValidateRequiredContacts(IEnumerable<object>? addresses, IEnumerable<object>? emails, IEnumerable<object>? phones, IEnumerable<object>? socialMedias)
        {
            if (!addresses?.Any() == true) throw new ArgumentException("At least one address is required", nameof(addresses));
            if (!emails?.Any() == true) throw new ArgumentException("At least one email is required", nameof(emails));
            if (!phones?.Any() == true) throw new ArgumentException("At least one phone is required", nameof(phones));
            if (!socialMedias?.Any() == true) throw new ArgumentException("At least one social media is required", nameof(socialMedias));
        }

        /// <summary>
        /// Procesa y agrega direcciones a la empresa, retornando diccionario de tipos
        /// </summary>
        public async Task<Dictionary<Guid, string>> ProcessAddressesAsync(Company company, IEnumerable<dynamic> addressDtos, CancellationToken cancellationToken)
        {
            var addressTypeNames = new Dictionary<Guid, string>();

            foreach (var addressDto in addressDtos)
            {
                var addressTypeEntity = await _addressTypeRepository.GetByIdAsync((Guid)addressDto.AddressTypeId, cancellationToken);
                if (addressTypeEntity == null)
                    throw new ArgumentException($"Address type with ID '{addressDto.AddressTypeId}' not found");

                // Agregar al diccionario para el resultado
                addressTypeNames[addressTypeEntity.Id] = addressTypeEntity.Name;

                var address = CompanyAddress.Create(company.Id, addressTypeEntity.Id, (string)addressDto.Address, (bool)addressDto.IsPrimary);
                company.AddAddress(address);
            }

            return addressTypeNames;
        }

        /// <summary>
        /// Procesa y agrega emails a la empresa, retornando diccionario de tipos
        /// </summary>
        public async Task<Dictionary<Guid, string>> ProcessEmailsAsync(Company company, IEnumerable<dynamic> emailDtos, CancellationToken cancellationToken)
        {
            var emailTypeNames = new Dictionary<Guid, string>();

            foreach (var emailDto in emailDtos)
            {
                var emailTypeEntity = await _emailTypeRepository.GetByIdAsync((Guid)emailDto.EmailTypeId, cancellationToken);
                if (emailTypeEntity == null)
                    throw new ArgumentException($"Email type with ID '{emailDto.EmailTypeId}' not found");

                // Agregar al diccionario para el resultado
                emailTypeNames[emailTypeEntity.Id] = emailTypeEntity.Name;

                var email = Email.Create((string)emailDto.Email);
                var companyEmail = CompanyEmail.Create(company.Id, emailTypeEntity.Id, email, (bool)emailDto.IsPrimary);
                company.AddEmail(companyEmail);
            }

            return emailTypeNames;
        }

        /// <summary>
        /// Procesa y agrega teléfonos a la empresa, retornando diccionario de tipos
        /// </summary>
        public async Task<Dictionary<Guid, string>> ProcessPhonesAsync(Company company, IEnumerable<dynamic> phoneDtos, CancellationToken cancellationToken)
        {
            var phoneTypeNames = new Dictionary<Guid, string>();

            foreach (var phoneDto in phoneDtos)
            {
                var phoneTypeEntity = await _phoneTypeRepository.GetByIdAsync((Guid)phoneDto.PhoneTypeId, cancellationToken);
                if (phoneTypeEntity == null)
                    throw new ArgumentException($"Phone type with ID '{phoneDto.PhoneTypeId}' not found");

                // Agregar al diccionario para el resultado
                phoneTypeNames[phoneTypeEntity.Id] = phoneTypeEntity.Name;

                var phone = CompanyPhone.Create(company.Id, phoneTypeEntity.Id, (string)phoneDto.Phone, (bool)phoneDto.IsPrimary);
                company.AddPhone(phone);
            }

            return phoneTypeNames;
        }

        /// <summary>
        /// Procesa y agrega redes sociales a la empresa, retornando diccionario de tipos
        /// </summary>
        public async Task<Dictionary<Guid, string>> ProcessSocialMediasAsync(Company company, IEnumerable<dynamic> socialMediaDtos, CancellationToken cancellationToken)
        {
            var socialMediaTypeNames = new Dictionary<Guid, string>();

            foreach (var socialMediaDto in socialMediaDtos)
            {
                var socialMediaTypeEntity = await _socialMediaTypeRepository.GetByIdAsync((Guid)socialMediaDto.SocialMediaTypeId, cancellationToken);
                if (socialMediaTypeEntity == null)
                    throw new ArgumentException($"Social media type with ID '{socialMediaDto.SocialMediaTypeId}' not found");

                // Agregar al diccionario para el resultado
                socialMediaTypeNames[socialMediaTypeEntity.Id] = socialMediaTypeEntity.Name;

                var socialMedia = CompanySocialMedia.Create(company.Id, socialMediaTypeEntity.Id, (string)socialMediaDto.Url, (bool)socialMediaDto.IsPrimary);
                company.AddSocialMedia(socialMedia);
            }

            return socialMediaTypeNames;
        }

        /// <summary>
        /// Procesa todos los tipos de contacto de una vez
        /// </summary>
        public async Task<ContactTypeNames> ProcessAllContactsAsync(Company company, IEnumerable<dynamic> addresses, IEnumerable<dynamic> emails, IEnumerable<dynamic> phones, IEnumerable<dynamic> socialMedias, CancellationToken cancellationToken)
        {
            // Procesar todos los contactos en paralelo para mejor performance
            var addressTask = ProcessAddressesAsync(company, addresses, cancellationToken);
            var emailTask = ProcessEmailsAsync(company, emails, cancellationToken);
            var phoneTask = ProcessPhonesAsync(company, phones, cancellationToken);
            var socialMediaTask = ProcessSocialMediasAsync(company, socialMedias, cancellationToken);

            await Task.WhenAll(addressTask, emailTask, phoneTask, socialMediaTask);

            return new ContactTypeNames(
                await addressTask,
                await emailTask,
                await phoneTask,
                await socialMediaTask
            );
        }

        /// <summary>
        /// Construye los resultados de direcciones
        /// </summary>
        public List<CompanyAddressResult> BuildAddressResults(Company company, Dictionary<Guid, string> addressTypeNames)
        {
            return company.Addresses.Select(a => new CompanyAddressResult(addressTypeNames[a.AddressTypeId], a.Address, a.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de emails
        /// </summary>
        public List<CompanyEmailResult> BuildEmailResults(Company company, Dictionary<Guid, string> emailTypeNames)
        {
            return company.Emails.Select(e => new CompanyEmailResult(emailTypeNames[e.EmailTypeId], e.Email.Value, e.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de teléfonos
        /// </summary>
        public List<CompanyPhoneResult> BuildPhoneResults(Company company, Dictionary<Guid, string> phoneTypeNames)
        {
            return company.Phones.Select(p => new CompanyPhoneResult(phoneTypeNames[p.PhoneTypeId], p.Phone, p.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de redes sociales
        /// </summary>
        public List<CompanySocialMediaResult> BuildSocialMediaResults(Company company, Dictionary<Guid, string> socialMediaTypeNames)
        {
            return company.SocialMedias.Select(sm => new CompanySocialMediaResult(socialMediaTypeNames[sm.SocialMediaTypeId], sm.Url, sm.IsPrimary)).ToList();
        }

        /// <summary>
        /// Construye los resultados de empleados
        /// </summary>
        public List<CompanyEmployeeResult> BuildEmployeeResults(Company company)
        {
            return company.Employees.Select(e => new CompanyEmployeeResult(e.FullName, e.Email, e.Phone, e.Position, e.HireDate)).ToList();
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
}
