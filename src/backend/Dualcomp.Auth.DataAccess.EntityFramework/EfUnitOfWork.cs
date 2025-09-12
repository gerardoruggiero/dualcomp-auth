using DualComp.Infraestructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework
{
	public sealed class EfUnitOfWork : IUnitOfWork
	{
		private readonly BaseDbContext _dbContext;

        public EfUnitOfWork(BaseDbContext dbContext) => _dbContext = dbContext;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
			=> _dbContext.SaveChangesAsync(cancellationToken);
	}
}
