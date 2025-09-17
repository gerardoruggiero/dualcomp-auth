using Dualcomp.Auth.Application.Companies.GetCompany;
using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Application.Companies.UpdateCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Users;

namespace Dualcomp.Auth.Application.Companies
{
    public interface ICompanyContactService
    {
        /// <summary>
        /// Valida que se proporcionen los contactos requeridos para registro
        /// </summary>
        void ValidateRequiredContactsForRegistration(
            IEnumerable<RegisterCompanyAddressDto>? addresses, 
            IEnumerable<RegisterCompanyEmailDto>? emails, 
            IEnumerable<RegisterCompanyPhoneDto>? phones, 
            IEnumerable<RegisterCompanySocialMediaDto>? socialMedias);

        /// <summary>
        /// Valida que se proporcionen los contactos requeridos para actualización
        /// </summary>
        void ValidateRequiredContactsForUpdate(
            IEnumerable<UpdateCompanyAddressDto>? addresses, 
            IEnumerable<UpdateCompanyEmailDto>? emails, 
            IEnumerable<UpdateCompanyPhoneDto>? phones, 
            IEnumerable<UpdateCompanySocialMediaDto>? socialMedias);

        /// <summary>
        /// Procesa todos los contactos de una empresa para registro (direcciones, emails, teléfonos, redes sociales)
        /// </summary>
        Task<ContactTypeNames> ProcessAllContactsForRegistrationAsync(
            Company company, 
            IEnumerable<RegisterCompanyAddressDto> addresses, 
            IEnumerable<RegisterCompanyEmailDto> emails, 
            IEnumerable<RegisterCompanyPhoneDto> phones, 
            IEnumerable<RegisterCompanySocialMediaDto> socialMedias, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Procesa todos los contactos de una empresa para actualización (direcciones, emails, teléfonos, redes sociales)
        /// </summary>
        Task<ContactTypeNames> ProcessAllContactsForUpdateAsync(
            Company company, 
            IEnumerable<UpdateCompanyAddressDto> addresses, 
            IEnumerable<UpdateCompanyEmailDto> emails, 
            IEnumerable<UpdateCompanyPhoneDto> phones, 
            IEnumerable<UpdateCompanySocialMediaDto> socialMedias, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Elimina contactos que ya no están en la request (fueron eliminados por el usuario)
        /// </summary>
        Task RemoveDeletedContactsAsync(
            Company company,
            IEnumerable<UpdateCompanyAddressDto> addresses,
            IEnumerable<UpdateCompanyEmailDto> emails,
            IEnumerable<UpdateCompanyPhoneDto> phones,
            IEnumerable<UpdateCompanySocialMediaDto> socialMedias,
            CancellationToken cancellationToken);

        /// <summary>
        /// Procesa empleados para actualización (pueden ser nuevos o existentes)
        /// </summary>
        Task ProcessEmployeesForUpdateAsync(
            Company company, 
            IEnumerable<UpdateCompanyEmployeeDto> employees, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Procesa empleados para registro (solo nuevos empleados)
        /// </summary>
        Task ProcessEmployeesForRegistrationAsync(
            Company company, 
            IEnumerable<RegisterCompanyEmployeeDto> employees, 
            CancellationToken cancellationToken);

        Task DeactivateDeletedEmployeesAsync(
            Company company,
            IEnumerable<UpdateCompanyEmployeeDto> employees,
            CancellationToken cancellationToken);

        /// <summary>
        /// Crea un usuario automáticamente para un empleado
        /// </summary>
        Task<User> CreateUserForEmployee(string fullName, string email, Guid companyId, CancellationToken cancellationToken);

        /// <summary>
        /// Construye los resultados de direcciones de la empresa
        /// </summary>
        List<CompanyAddressResult> BuildAddressResults(Company company, Dictionary<Guid, string> addressTypeNames);

        /// <summary>
        /// Construye los resultados de emails de la empresa
        /// </summary>
        List<CompanyEmailResult> BuildEmailResults(Company company, Dictionary<Guid, string> emailTypeNames);

        /// <summary>
        /// Construye los resultados de teléfonos de la empresa
        /// </summary>
        List<CompanyPhoneResult> BuildPhoneResults(Company company, Dictionary<Guid, string> phoneTypeNames);

        /// <summary>
        /// Construye los resultados de redes sociales de la empresa
        /// </summary>
        List<CompanySocialMediaResult> BuildSocialMediaResults(Company company, Dictionary<Guid, string> socialMediaTypeNames);

        /// <summary>
        /// Construye los resultados de empleados de la empresa
        /// </summary>
        List<CompanyEmployeeResult> BuildEmployeeResults(Company company);
    }
}
