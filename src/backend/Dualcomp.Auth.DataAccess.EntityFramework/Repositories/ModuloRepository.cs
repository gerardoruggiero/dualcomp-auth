using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class ModuloRepository : EfRepository<ModuloEntity>, IModuloRepository
	{
		public ModuloRepository(IDbContextFactory<BaseDbContext> dbContextFactory, BaseDbContext dbContext) 
			: base(dbContextFactory, dbContext)
		{
		}

		public async Task<IEnumerable<ModuloEntity>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await ListAsync(cancellationToken);
		}
	}
}
