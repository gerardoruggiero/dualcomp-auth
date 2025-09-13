namespace Dualcomp.Auth.Domain.Users.Repositories
{
    public interface IEmailValidationRepository
    {
        Task<EmailValidation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<EmailValidation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<EmailValidation?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailValidation>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<EmailValidation>> GetUnusedTokensAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<bool> HasActiveTokenAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(EmailValidation emailValidation, CancellationToken cancellationToken = default);
        Task UpdateAsync(EmailValidation emailValidation, CancellationToken cancellationToken = default);
        Task DeleteAsync(EmailValidation emailValidation, CancellationToken cancellationToken = default);
        Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);
        Task<int> CountActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}

