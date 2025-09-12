using Microsoft.EntityFrameworkCore;
using Dualcomp.Auth.Domain.Users;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly BaseDbContext _context;

        public UserSessionRepository(BaseDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<UserSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<UserSession?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(s => s.AccessToken == accessToken && s.IsActive, cancellationToken);
        }

        public async Task<UserSession?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .Where(s => s.UserId == userId && s.IsActive && s.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<UserSession>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .Where(s => s.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
        {
            await _context.UserSessions.AddAsync(session, cancellationToken);
        }

        public Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default)
        {
            _context.UserSessions.Update(session);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(UserSession session, CancellationToken cancellationToken = default)
        {
            _context.UserSessions.Remove(session);
            return Task.CompletedTask;
        }

        public async Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default)
        {
            var expiredSessions = await GetExpiredSessionsAsync(cancellationToken);
            _context.UserSessions.RemoveRange(expiredSessions);
        }

        public async Task DeactivateAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var activeSessions = await GetActiveSessionsByUserIdAsync(userId, cancellationToken);
            foreach (var session in activeSessions)
            {
                session.Deactivate();
            }
            _context.UserSessions.UpdateRange(activeSessions);
        }
    }
}
