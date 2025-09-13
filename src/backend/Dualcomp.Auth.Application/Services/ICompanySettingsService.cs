using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Services
{
    public interface ICompanySettingsService
    {
        /// <summary>
        /// Crea configuración SMTP por defecto para una empresa
        /// </summary>
        Task<CompanySettings> CreateDefaultSmtpSettingsAsync(
            Guid companyId, 
            Guid? createdBy = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene la configuración SMTP activa para una empresa
        /// </summary>
        Task<CompanySettings?> GetActiveSmtpSettingsAsync(
            Guid companyId, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene la configuración SMTP activa o crea una por defecto si no existe
        /// </summary>
        Task<CompanySettings> GetOrCreateDefaultSmtpSettingsAsync(
            Guid companyId, 
            Guid? createdBy = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza la configuración SMTP de una empresa
        /// </summary>
        Task<CompanySettings> UpdateSmtpSettingsAsync(
            Guid companyId,
            string smtpServer,
            int smtpPort,
            string smtpUsername,
            string smtpPassword,
            bool smtpUseSsl,
            string smtpFromEmail,
            string smtpFromName,
            Guid? updatedBy = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Desactiva la configuración SMTP de una empresa
        /// </summary>
        Task DeactivateSmtpSettingsAsync(
            Guid companyId, 
            CancellationToken cancellationToken = default);
    }
}
