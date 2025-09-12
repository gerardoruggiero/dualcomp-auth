using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class EmailTypeRepository : EfRepository<EmailTypeEntity>, IEmailTypeRepository
	{
        public EmailTypeRepository(IDbContextFactory<BaseDbContext> dbContextFactory, BaseDbContext dbContext) 
			: base(dbContextFactory, dbContext)
        {
        }
	}
}
