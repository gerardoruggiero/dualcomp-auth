using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class SocialMediaTypeRepository : EfRepository<SocialMediaTypeEntity>, ISocialMediaTypeRepository
	{
		private readonly BaseDbContext _dbContext;

        public SocialMediaTypeRepository(BaseDbContext dbContext) : base(dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<SocialMediaTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbContext.Set<SocialMediaTypeEntity>()
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}
	}
}
