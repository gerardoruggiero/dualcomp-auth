using Dualcomp.Auth.Application.Companies.GetCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Users;

namespace Dualcomp.Auth.Application.Companies
{
    public interface ICompanyContactService
    {
        /// <summary>
        /// Valida que se proporcionen los contactos requeridos
        /// </summary>
        void ValidateRequiredContacts(IEnumerable<object>? addresses, IEnumerable<object>? emails, IEnumerable<object>? phones, IEnumerable<object>? socialMedias);

        /// <summary>
        /// Procesa todos los contactos de una empresa (direcciones, emails, teléfonos, redes sociales)
        /// </summary>
        Task<ContactTypeNames> ProcessAllContactsAsync(
            Company company, 
            IEnumerable<dynamic> addresses, 
            IEnumerable<dynamic> emails, 
            IEnumerable<dynamic> phones, 
            IEnumerable<dynamic> socialMedias, 
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
