using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
    public class EmailValidationRepository : IEmailValidationRepository
    {
        private readonly BaseDbContext _context;

        public EmailValidationRepository(BaseDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<EmailValidation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<EmailValidation>()
                .FirstOrDefaultAsync(ev => ev.Id == id, cancellationToken);
        }

        public async Task<EmailValidation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.Set<EmailValidation>()
                .FirstOrDefaultAsync(ev => ev.Token == token, cancellationToken);
        }

        public async Task<EmailValidation?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<EmailValidation>()
                .Where(ev => ev.UserId == userId)
                .OrderByDescending(ev => ev.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<EmailValidation>> GetExpiredTokensAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await _context.Set<EmailValidation>()
                .Where(ev => ev.ExpiresAt < now)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<EmailValidation>> GetUnusedTokensAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<EmailValidation>()
                .Where(ev => !ev.IsUsed)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.Set<EmailValidation>()
                .AnyAsync(ev => ev.Token == token, cancellationToken);
        }

        public async Task<bool> HasActiveTokenAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await _context.Set<EmailValidation>()
                .AnyAsync(ev => ev.UserId == userId && !ev.IsUsed && ev.ExpiresAt > now, cancellationToken);
        }

        public async Task AddAsync(EmailValidation emailValidation, CancellationToken cancellationToken = default)
        {
            await _context.Set<EmailValidation>().AddAsync(emailValidation, cancellationToken);
        }

        public async Task UpdateAsync(EmailValidation emailValidation, CancellationToken cancellationToken = default)
        {
            _context.Set<EmailValidation>().Update(emailValidation);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(EmailValidation emailValidation, CancellationToken cancellationToken = default)
        {
            _context.Set<EmailValidation>().Remove(emailValidation);
            await Task.CompletedTask;
        }

        public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var expiredTokens = await _context.Set<EmailValidation>()
                .Where(ev => ev.ExpiresAt < now)
                .ToListAsync(cancellationToken);

            _context.Set<EmailValidation>().RemoveRange(expiredTokens);
        }

        public async Task<int> CountActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await _context.Set<EmailValidation>()
                .CountAsync(ev => ev.UserId == userId && !ev.IsUsed && ev.ExpiresAt > now, cancellationToken);
        }
    }
}
