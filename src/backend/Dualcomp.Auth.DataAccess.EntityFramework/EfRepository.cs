using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Domain.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dualcomp.Auth.DataAccess.EntityFramework
{
	public class EfRepository<TAggregate> : IRepository<TAggregate>
		where TAggregate : Entity
    {
		protected readonly IDbContextFactory<BaseDbContext> DbContextFactory;
		protected readonly BaseDbContext DbContext;

		public EfRepository(IDbContextFactory<BaseDbContext> dbContextFactory, BaseDbContext dbContext)
		{
			DbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
			DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		protected virtual async Task<BaseDbContext> CreateContextAsync(CancellationToken cancellationToken = default)
		{
			return await DbContextFactory.CreateDbContextAsync(cancellationToken);
		}

		protected virtual BaseDbContext CreateContext()
		{
			return DbContextFactory.CreateDbContext();
		}

		public virtual async Task<TAggregate?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
		{
			return await DbContext.Set<TAggregate>().FindAsync([id], cancellationToken);
		}

		public virtual async Task<IReadOnlyList<TAggregate>> ListAsync(CancellationToken cancellationToken = default)
		{
			using var context = CreateContext();
			return await context.Set<TAggregate>().AsNoTracking().ToListAsync(cancellationToken);
		}

		public virtual async Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			using var context = CreateContext();
			return await context.Set<TAggregate>().AsNoTracking().ToListAsync(cancellationToken);
		}

		public virtual async Task<IReadOnlyList<TAggregate>> ListAsync(Expression<Func<TAggregate, bool>> predicate, CancellationToken cancellationToken = default)
		{
			using var context = CreateContext();
			return await context.Set<TAggregate>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
		}

		public virtual async Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default)
		{
			await DbContext.Set<TAggregate>().AddAsync(entity, cancellationToken);
		}

		public virtual async Task AddRangeAsync(IEnumerable<TAggregate> entities, CancellationToken cancellationToken = default)
		{
			await DbContext.Set<TAggregate>().AddRangeAsync(entities, cancellationToken);
		}

        public virtual async Task<TAggregate> UpdateAsync(TAggregate item, CancellationToken cancellationToken = default)
        {
            try
            {
                var entry = DbContext.Entry(item);
                if (entry.State == EntityState.Detached)
                {
                    var existingEntity = await DbContext.Set<TAggregate>().Where(w => w.Id == item.Id).SingleAsync();
                    if (existingEntity == null)
                    {
                        throw new KeyNotFoundException("Entity not found in the database");
                    }

                    DbContext.Entry(existingEntity).CurrentValues.SetValues(item);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving the entity", ex);
            }
        }

        public virtual void Update(TAggregate entity)
		{
			DbContext.Set<TAggregate>().Update(entity);
		}

		public virtual void Remove(TAggregate entity)
		{
			DbContext.Set<TAggregate>().Remove(entity);
		}
	}
}
