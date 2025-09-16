using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class CompanyRepository : EfRepository<Company>, ICompanyRepository
	{
		private readonly BaseDbContext _dbContext;

        public CompanyRepository(IDbContextFactory<BaseDbContext> dbContextFactory, BaseDbContext dbContext) 
			: base(dbContextFactory, dbContext) => _dbContext = dbContext;

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

        /// <summary>
        /// Obtiene una empresa con todos sus contactos y tipos relacionados usando includes optimizados
        /// </summary>
        public async Task<CompanyWithTypes?> GetByIdWithTypesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var company = await _dbContext.Companies
                .Include(c => c.Employees)
                .Include(c => c.Addresses)
                .Include(c => c.Emails)
                .Include(c => c.Phones)
                .Include(c => c.SocialMedias)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (company == null) return null;

            // Obtener todos los tipos únicos necesarios
            var addressTypeIds = company.Addresses.Select(a => a.AddressTypeId).Distinct().ToList();
            var emailTypeIds = company.Emails.Select(e => e.EmailTypeId).Distinct().ToList();
            var phoneTypeIds = company.Phones.Select(p => p.PhoneTypeId).Distinct().ToList();
            var socialMediaTypeIds = company.SocialMedias.Select(sm => sm.SocialMediaTypeId).Distinct().ToList();

            // Cargar todos los tipos en paralelo
            var addressTypesTask = _dbContext.AddressTypes
                .Where(at => addressTypeIds.Contains(at.Id))
                .ToDictionaryAsync(at => at.Id, at => at.Name, cancellationToken);

            var emailTypesTask = _dbContext.EmailTypes
                .Where(et => emailTypeIds.Contains(et.Id))
                .ToDictionaryAsync(et => et.Id, et => et.Name, cancellationToken);

            var phoneTypesTask = _dbContext.PhoneTypes
                .Where(pt => phoneTypeIds.Contains(pt.Id))
                .ToDictionaryAsync(pt => pt.Id, pt => pt.Name, cancellationToken);

            var socialMediaTypesTask = _dbContext.SocialMediaTypes
                .Where(smt => socialMediaTypeIds.Contains(smt.Id))
                .ToDictionaryAsync(smt => smt.Id, smt => smt.Name, cancellationToken);

            await Task.WhenAll(addressTypesTask, emailTypesTask, phoneTypesTask, socialMediaTypesTask);

            return new CompanyWithTypes(
                company,
                await addressTypesTask,
                await emailTypesTask,
                await phoneTypesTask,
                await socialMediaTypesTask
            );
        }

		public override async Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbContext.Companies
				.Include(c => c.Employees)
				.Include(c => c.Addresses)
				.Include(c => c.Emails)
				.Include(c => c.Phones)
				.Include(c => c.SocialMedias)
				.ToListAsync(cancellationToken);
		}

		public override async Task<IReadOnlyList<Company>> ListAsync(CancellationToken cancellationToken = default)
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
