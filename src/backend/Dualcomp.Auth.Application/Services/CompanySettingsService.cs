using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Services
{
    public class CompanySettingsService
    {
        private readonly ICompanySettingsRepository _companySettingsRepository;

        public CompanySettingsService(ICompanySettingsRepository companySettingsRepository)
        {
            _companySettingsRepository = companySettingsRepository;
        }

        /// <summary>
        /// Crea configuración SMTP por defecto para una empresa
        /// </summary>
        public async Task<CompanySettings> CreateDefaultSmtpSettingsAsync(
            Guid companyId, 
            Guid? createdBy = null, 
            CancellationToken cancellationToken = default)
        {
            // Verificar si ya existe configuración para esta empresa
            var existingSettings = await _companySettingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
            if (existingSettings != null)
            {
                throw new InvalidOperationException("La empresa ya tiene configuración SMTP");
            }

            // Crear configuración por defecto
            var defaultSettings = CompanySettings.Create(
                companyId: companyId,
                smtpServer: "smtp.gmail.com",
                smtpPort: 587,
                smtpUsername: "noreply@dualcomp.com",
                smtpPassword: "ENCRYPTED_PASSWORD_HERE", // Debe ser encriptado
                smtpUseSsl: true,
                smtpFromEmail: "noreply@dualcomp.com",
                smtpFromName: "DualComp CRM",
                createdBy: createdBy
            );

            await _companySettingsRepository.AddAsync(defaultSettings, cancellationToken);
            return defaultSettings;
        }

        /// <summary>
        /// Obtiene la configuración SMTP activa para una empresa
        /// </summary>
        public async Task<CompanySettings?> GetActiveSmtpSettingsAsync(
            Guid companyId, 
            CancellationToken cancellationToken = default)
        {
            return await _companySettingsRepository.GetActiveByCompanyIdAsync(companyId, cancellationToken);
        }

        /// <summary>
        /// Obtiene la configuración SMTP activa o crea una por defecto si no existe
        /// </summary>
        public async Task<CompanySettings> GetOrCreateDefaultSmtpSettingsAsync(
            Guid companyId, 
            Guid? createdBy = null, 
            CancellationToken cancellationToken = default)
        {
            var existingSettings = await GetActiveSmtpSettingsAsync(companyId, cancellationToken);
            
            if (existingSettings != null)
            {
                return existingSettings;
            }

            return await CreateDefaultSmtpSettingsAsync(companyId, createdBy, cancellationToken);
        }

        /// <summary>
        /// Actualiza la configuración SMTP de una empresa
        /// </summary>
        public async Task<CompanySettings> UpdateSmtpSettingsAsync(
            Guid companyId,
            string smtpServer,
            int smtpPort,
            string smtpUsername,
            string smtpPassword,
            bool smtpUseSsl,
            string smtpFromEmail,
            string smtpFromName,
            Guid? updatedBy = null,
            CancellationToken cancellationToken = default)
        {
            var settings = await _companySettingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
            
            if (settings == null)
            {
                throw new InvalidOperationException("No se encontró configuración SMTP para esta empresa");
            }

            settings.UpdateSettings(
                smtpServer, smtpPort, smtpUsername, smtpPassword,
                smtpUseSsl, smtpFromEmail, smtpFromName, updatedBy);

            await _companySettingsRepository.UpdateAsync(settings, cancellationToken);
            return settings;
        }

        /// <summary>
        /// Desactiva la configuración SMTP de una empresa
        /// </summary>
        public async Task DeactivateSmtpSettingsAsync(
            Guid companyId, 
            CancellationToken cancellationToken = default)
        {
            await _companySettingsRepository.DeactivateByCompanyIdAsync(companyId, cancellationToken);
        }
    }
}
