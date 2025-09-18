using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class TitleRepository : EfRepository<TitleEntity>, ITitleRepository
	{
        public TitleRepository(IDbContextFactory<BaseDbContext> dbContextFactory, BaseDbContext dbContext) 
			: base(dbContextFactory, dbContext)
        {
        }
	}
}

