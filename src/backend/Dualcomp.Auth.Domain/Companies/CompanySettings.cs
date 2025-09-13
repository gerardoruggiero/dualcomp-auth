using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
    public class CompanySettings : Entity
    {
        public Guid CompanyId { get; private set; }
        public string SmtpServer { get; private set; } = string.Empty;
        public int SmtpPort { get; private set; }
        public string SmtpUsername { get; private set; } = string.Empty;
        public string SmtpPassword { get; private set; } = string.Empty; // Encriptado
        public bool SmtpUseSsl { get; private set; }
        public string SmtpFromEmail { get; private set; } = string.Empty;
        public string SmtpFromName { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public Guid? CreatedBy { get; private set; }
        public Guid? UpdatedBy { get; private set; }

        private CompanySettings() { }

        private CompanySettings(
            Guid companyId,
            string smtpServer,
            int smtpPort,
            string smtpUsername,
            string smtpPassword,
            bool smtpUseSsl,
            string smtpFromEmail,
            string smtpFromName,
            Guid? createdBy = null)
        {
            Id = Guid.NewGuid();
            CompanyId = companyId;
            SmtpServer = string.IsNullOrWhiteSpace(smtpServer) ? throw new ArgumentException("SMTP Server is required", nameof(smtpServer)) : smtpServer.Trim();
            SmtpPort = smtpPort > 0 ? smtpPort : throw new ArgumentException("SMTP Port must be greater than 0", nameof(smtpPort));
            SmtpUsername = string.IsNullOrWhiteSpace(smtpUsername) ? throw new ArgumentException("SMTP Username is required", nameof(smtpUsername)) : smtpUsername.Trim();
            SmtpPassword = string.IsNullOrWhiteSpace(smtpPassword) ? throw new ArgumentException("SMTP Password is required", nameof(smtpPassword)) : smtpPassword;
            SmtpUseSsl = smtpUseSsl;
            SmtpFromEmail = string.IsNullOrWhiteSpace(smtpFromEmail) ? throw new ArgumentException("From Email is required", nameof(smtpFromEmail)) : smtpFromEmail.Trim();
            SmtpFromName = string.IsNullOrWhiteSpace(smtpFromName) ? throw new ArgumentException("From Name is required", nameof(smtpFromName)) : smtpFromName.Trim();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public static CompanySettings Create(
            Guid companyId,
            string smtpServer,
            int smtpPort,
            string smtpUsername,
            string smtpPassword,
            bool smtpUseSsl,
            string smtpFromEmail,
            string smtpFromName,
            Guid? createdBy = null)
            => new CompanySettings(companyId, smtpServer, smtpPort, smtpUsername, smtpPassword, smtpUseSsl, smtpFromEmail, smtpFromName, createdBy);

        public void UpdateSettings(
            string smtpServer,
            int smtpPort,
            string smtpUsername,
            string smtpPassword,
            bool smtpUseSsl,
            string smtpFromEmail,
            string smtpFromName,
            Guid? updatedBy = null)
        {
            SmtpServer = string.IsNullOrWhiteSpace(smtpServer) ? throw new ArgumentException("SMTP Server is required", nameof(smtpServer)) : smtpServer.Trim();
            SmtpPort = smtpPort > 0 ? smtpPort : throw new ArgumentException("SMTP Port must be greater than 0", nameof(smtpPort));
            SmtpUsername = string.IsNullOrWhiteSpace(smtpUsername) ? throw new ArgumentException("SMTP Username is required", nameof(smtpUsername)) : smtpUsername.Trim();
            SmtpPassword = string.IsNullOrWhiteSpace(smtpPassword) ? throw new ArgumentException("SMTP Password is required", nameof(smtpPassword)) : smtpPassword;
            SmtpUseSsl = smtpUseSsl;
            SmtpFromEmail = string.IsNullOrWhiteSpace(smtpFromEmail) ? throw new ArgumentException("From Email is required", nameof(smtpFromEmail)) : smtpFromEmail.Trim();
            SmtpFromName = string.IsNullOrWhiteSpace(smtpFromName) ? throw new ArgumentException("From Name is required", nameof(smtpFromName)) : smtpFromName.Trim();
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsValidConfiguration()
        {
            return !string.IsNullOrWhiteSpace(SmtpServer) &&
                   SmtpPort > 0 &&
                   !string.IsNullOrWhiteSpace(SmtpUsername) &&
                   !string.IsNullOrWhiteSpace(SmtpPassword) &&
                   !string.IsNullOrWhiteSpace(SmtpFromEmail) &&
                   !string.IsNullOrWhiteSpace(SmtpFromName);
        }
    }
}

