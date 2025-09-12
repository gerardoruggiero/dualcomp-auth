using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class PhoneTypeRepository : EfRepository<PhoneTypeEntity>, IPhoneTypeRepository
	{
		private readonly BaseDbContext _dbContext;

        public PhoneTypeRepository(BaseDbContext dbContext) : base(dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<PhoneTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbContext.Set<PhoneTypeEntity>()
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}
	}
}
