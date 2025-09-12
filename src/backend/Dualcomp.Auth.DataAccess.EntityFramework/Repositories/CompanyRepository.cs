using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class CompanyRepository : EfRepository<Company>, ICompanyRepository
	{
		private readonly BaseDbContext _dbContext;

        public CompanyRepository(BaseDbContext dbContext) : base(dbContext) => _dbContext = dbContext;

        public Task<bool> ExistsByTaxIdAsync(string normalizedTaxId, CancellationToken cancellationToken = default)
		{
			return _dbContext.Set<Company>()
				.AsNoTracking()
				.AnyAsync(c => c.TaxId.Value == normalizedTaxId, cancellationToken);
		}

		public Task<bool> ExistsByTaxIdForOtherCompanyAsync(string normalizedTaxId, Guid excludeCompanyId, CancellationToken cancellationToken = default)
		{
			return _dbContext.Set<Company>()
				.AsNoTracking()
				.AnyAsync(c => c.TaxId.Value == normalizedTaxId && c.Id != excludeCompanyId, cancellationToken);
		}

		public async Task<Company?> GetByTaxIdAsync(TaxId taxId, CancellationToken cancellationToken = default)
		{
			return await _dbContext.Companies
				.Include(c => c.Employees)
				.Include(c => c.Addresses)
				.Include(c => c.Emails)
				.Include(c => c.Phones)
				.Include(c => c.SocialMedias)
				.FirstOrDefaultAsync(c => c.TaxId.Value == taxId.Value, cancellationToken);
		}

		public async Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _dbContext.Companies
				.Include(c => c.Employees)
				.Include(c => c.Addresses)
				.Include(c => c.Emails)
				.Include(c => c.Phones)
				.Include(c => c.SocialMedias)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		public async Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbContext.Companies
				.Include(c => c.Employees)
				.Include(c => c.Addresses)
				.Include(c => c.Emails)
				.Include(c => c.Phones)
				.Include(c => c.SocialMedias)
				.ToListAsync(cancellationToken);
		}

		public async Task<IEnumerable<Company>> ListAsync(CancellationToken cancellationToken = default)
		{
			return await _dbContext.Companies
				.Include(c => c.Employees)
				.Include(c => c.Addresses)
				.Include(c => c.Emails)
				.Include(c => c.Phones)
				.Include(c => c.SocialMedias)
				.ToListAsync(cancellationToken);
		}

		public Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
		{
			_dbContext.Companies.Update(company);
			return Task.CompletedTask;
		}
	}
}
