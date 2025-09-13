using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.Domain.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);
        
        // Métodos para gestión de usuarios con paginación
        Task<IEnumerable<User>> GetUsersAsync(Guid? companyId, int page, int pageSize, string? searchTerm, CancellationToken cancellationToken = default);
        Task<int> GetUsersCountAsync(Guid? companyId, string? searchTerm, CancellationToken cancellationToken = default);
    }
}
