namespace Dualcomp.Auth.Domain.Companies.Repositories
{
    public interface IEmailLogRepository
    {
        Task<EmailLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailLog>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailLog>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailLog>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailLog>> GetByEmailTypeAsync(string emailType, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailLog>> GetFailedEmailsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailLog>> GetPendingEmailsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailLog>> GetEmailsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<int> GetEmailCountByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<int> GetEmailCountByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task AddAsync(EmailLog emailLog, CancellationToken cancellationToken = default);
        Task UpdateAsync(EmailLog emailLog, CancellationToken cancellationToken = default);
        Task DeleteAsync(EmailLog emailLog, CancellationToken cancellationToken = default);
        Task DeleteOldLogsAsync(int daysOld, CancellationToken cancellationToken = default);
    }
}

