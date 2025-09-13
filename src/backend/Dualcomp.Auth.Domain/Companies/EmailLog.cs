using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
    public class EmailLog : Entity
    {
        public Guid CompanyId { get; private set; }
        public string ToEmail { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string EmailType { get; private set; } = string.Empty;
        public string Status { get; private set; } = string.Empty;
        public string? ErrorMessage { get; private set; }
        public DateTime? SentAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private EmailLog() { }

        private EmailLog(
            Guid companyId,
            string toEmail,
            string subject,
            string emailType,
            string status,
            string? errorMessage = null)
        {
            Id = Guid.NewGuid();
            CompanyId = companyId;
            ToEmail = string.IsNullOrWhiteSpace(toEmail) ? throw new ArgumentException("To Email is required", nameof(toEmail)) : toEmail.Trim();
            Subject = string.IsNullOrWhiteSpace(subject) ? throw new ArgumentException("Subject is required", nameof(subject)) : subject.Trim();
            EmailType = string.IsNullOrWhiteSpace(emailType) ? throw new ArgumentException("Email Type is required", nameof(emailType)) : emailType.Trim();
            Status = string.IsNullOrWhiteSpace(status) ? throw new ArgumentException("Status is required", nameof(status)) : status.Trim();
            ErrorMessage = errorMessage;
            CreatedAt = DateTime.UtcNow;
        }

        public static EmailLog Create(
            Guid companyId,
            string toEmail,
            string subject,
            string emailType,
            string status,
            string? errorMessage = null)
            => new EmailLog(companyId, toEmail, subject, emailType, status, errorMessage);

        public void MarkAsSent()
        {
            if (Status == "Sent")
                throw new InvalidOperationException("Email has already been marked as sent");

            Status = "Sent";
            SentAt = DateTime.UtcNow;
            ErrorMessage = null;
        }

        public void MarkAsFailed(string errorMessage)
        {
            if (Status == "Sent")
                throw new InvalidOperationException("Cannot mark as failed an email that was already sent");

            Status = "Failed";
            ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "Unknown error" : errorMessage.Trim();
        }

        public void MarkAsPending()
        {
            Status = "Pending";
            ErrorMessage = null;
        }

        public bool IsSent => Status == "Sent";
        public bool IsFailed => Status == "Failed";
        public bool IsPending => Status == "Pending";
    }
}

