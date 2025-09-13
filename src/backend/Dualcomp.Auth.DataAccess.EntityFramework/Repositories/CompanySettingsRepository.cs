using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
    public class CompanySettingsRepository : ICompanySettingsRepository
    {
        private readonly BaseDbContext _context;

        public CompanySettingsRepository(BaseDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CompanySettings?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<CompanySettings>()
                .FirstOrDefaultAsync(cs => cs.Id == id, cancellationToken);
        }

        public async Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<CompanySettings>()
                .FirstOrDefaultAsync(cs => cs.CompanyId == companyId, cancellationToken);
        }

        public async Task<CompanySettings?> GetActiveByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<CompanySettings>()
                .Where(cs => cs.CompanyId == companyId && cs.IsActive)
                .OrderByDescending(cs => cs.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompanySettings>> GetActiveSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<CompanySettings>()
                .Where(cs => cs.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<CompanySettings>()
                .AnyAsync(cs => cs.CompanyId == companyId, cancellationToken);
        }

        public async Task AddAsync(CompanySettings companySettings, CancellationToken cancellationToken = default)
        {
            await _context.Set<CompanySettings>().AddAsync(companySettings, cancellationToken);
        }

        public async Task UpdateAsync(CompanySettings companySettings, CancellationToken cancellationToken = default)
        {
            _context.Set<CompanySettings>().Update(companySettings);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(CompanySettings companySettings, CancellationToken cancellationToken = default)
        {
            _context.Set<CompanySettings>().Remove(companySettings);
            await Task.CompletedTask;
        }

        public async Task DeactivateByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var settings = await _context.Set<CompanySettings>()
                .Where(cs => cs.CompanyId == companyId && cs.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var setting in settings)
            {
                setting.Deactivate();
                _context.Set<CompanySettings>().Update(setting);
            }
        }
    }
}
