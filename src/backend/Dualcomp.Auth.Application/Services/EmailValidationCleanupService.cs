using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.Application.Services
{
    public class EmailValidationCleanupService
    {
        private readonly IEmailValidationRepository _emailValidationRepository;
        private readonly IEmailLogRepository _emailLogRepository;

        public EmailValidationCleanupService(
            IEmailValidationRepository emailValidationRepository,
            IEmailLogRepository emailLogRepository)
        {
            _emailValidationRepository = emailValidationRepository;
            _emailLogRepository = emailLogRepository;
        }

        /// <summary>
        /// Limpia tokens de validación expirados
        /// </summary>
        public async Task<int> CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var expiredTokens = await _emailValidationRepository.GetExpiredTokensAsync(cancellationToken);
                var count = 0;

                foreach (var token in expiredTokens)
                {
                    await _emailValidationRepository.DeleteAsync(token, cancellationToken);
                    count++;
                }

                return count;
            }
            catch (Exception)
            {
                // Log error but don't throw to avoid breaking the application
                return 0;
            }
        }

        /// <summary>
        /// Limpia logs de email antiguos (más de 90 días)
        /// </summary>
        public async Task<int> CleanupOldEmailLogsAsync(int daysOld = 90, CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                var oldLogs = await _emailLogRepository.GetEmailsByDateRangeAsync(
                    DateTime.MinValue, cutoffDate, cancellationToken);
                
                var count = 0;
                foreach (var log in oldLogs)
                {
                    await _emailLogRepository.DeleteAsync(log, cancellationToken);
                    count++;
                }

                return count;
            }
            catch (Exception)
            {
                // Log error but don't throw to avoid breaking the application
                return 0;
            }
        }

        /// <summary>
        /// Ejecuta todas las tareas de limpieza
        /// </summary>
        public async Task<CleanupResult> RunCleanupAsync(CancellationToken cancellationToken = default)
        {
            var expiredTokensCount = await CleanupExpiredTokensAsync(cancellationToken);
            var oldLogsCount = await CleanupOldEmailLogsAsync(90, cancellationToken);

            return new CleanupResult
            {
                ExpiredTokensDeleted = expiredTokensCount,
                OldLogsDeleted = oldLogsCount,
                ExecutedAt = DateTime.UtcNow
            };
        }
    }

    public class CleanupResult
    {
        public int ExpiredTokensDeleted { get; set; }
        public int OldLogsDeleted { get; set; }
        public DateTime ExecutedAt { get; set; }
    }
}

