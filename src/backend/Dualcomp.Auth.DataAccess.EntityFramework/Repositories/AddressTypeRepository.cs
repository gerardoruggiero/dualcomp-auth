using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class AddressTypeRepository : EfRepository<AddressTypeEntity>, IAddressTypeRepository
	{
		private readonly BaseDbContext _dbContext;

        public AddressTypeRepository(BaseDbContext dbContext) : base(dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<AddressTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbContext.Set<AddressTypeEntity>()
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}
	}
}
