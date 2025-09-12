using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class EmailTypeRepository : EfRepository<EmailTypeEntity>, IEmailTypeRepository
	{
		private readonly BaseDbContext _dbContext;

        public EmailTypeRepository(BaseDbContext dbContext) : base(dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<EmailTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbContext.Set<EmailTypeEntity>()
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}
	}
}
