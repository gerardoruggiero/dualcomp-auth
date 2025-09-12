using System.Linq.Expressions;
using DualComp.Infraestructure.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dualcomp.Auth.DataAccess.EntityFramework
{
	public class EfRepository<TAggregate> : IRepository<TAggregate>
		where TAggregate : class
	{
		protected readonly BaseDbContext DbContext;
		protected readonly DbSet<TAggregate> Set;

	public EfRepository(BaseDbContext dbContext)
	{
		DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		Set = dbContext.Set<TAggregate>();
	}

		public virtual async Task<TAggregate?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
		{
			return await Set.FindAsync([id], cancellationToken);
		}

		public virtual async Task<IReadOnlyList<TAggregate>> ListAsync(CancellationToken cancellationToken = default)
		{
			return await Set.AsNoTracking().ToListAsync(cancellationToken);
		}

		public virtual async Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await Set.AsNoTracking().ToListAsync(cancellationToken);
		}

		public virtual async Task<IReadOnlyList<TAggregate>> ListAsync(Expression<Func<TAggregate, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await Set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
		}

		public virtual async Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default)
		{
			await Set.AddAsync(entity, cancellationToken);
		}

		public virtual async Task AddRangeAsync(IEnumerable<TAggregate> entities, CancellationToken cancellationToken = default)
		{
			await Set.AddRangeAsync(entities, cancellationToken);
		}

		public virtual void Update(TAggregate entity)
		{
			Set.Update(entity);
		}

		public virtual void Remove(TAggregate entity)
		{
			Set.Remove(entity);
		}
	}
}
