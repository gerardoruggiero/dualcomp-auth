using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class SocialMediaTypeRepository : EfRepository<SocialMediaTypeEntity>, ISocialMediaTypeRepository
	{
        public SocialMediaTypeRepository(BaseDbContext dbContext) : base(dbContext)
        {
        }
	}
}
