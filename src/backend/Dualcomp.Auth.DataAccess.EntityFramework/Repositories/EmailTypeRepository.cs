using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class EmailTypeRepository : EfRepository<EmailTypeEntity>, IEmailTypeRepository
	{
        public EmailTypeRepository(BaseDbContext dbContext) : base(dbContext)
        {
        }
	}
}
