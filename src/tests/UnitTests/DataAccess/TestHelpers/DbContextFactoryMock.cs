using Dualcomp.Auth.DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.UnitTests.DataAccess.TestHelpers
{
    public class DbContextFactoryMock : IDbContextFactory<BaseDbContext>
    {
        private readonly BaseDbContext _context;

        public DbContextFactoryMock(BaseDbContext context)
        {
            _context = context;
        }

        public BaseDbContext CreateDbContext()
        {
            return _context;
        }

        public Task<BaseDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_context);
        }
    }
}
