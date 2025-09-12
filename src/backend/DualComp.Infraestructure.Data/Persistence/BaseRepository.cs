using System.Linq.Expressions;

namespace DualComp.Infraestructure.Data.Persistence
{
	public abstract class BaseRepository<TAggregate> : IRepository<TAggregate>
		where TAggregate : class
	{
		public abstract Task<TAggregate?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
		public abstract Task<IReadOnlyList<TAggregate>> ListAsync(CancellationToken cancellationToken = default);
		public abstract Task<IReadOnlyList<TAggregate>> ListAsync(Expression<Func<TAggregate, bool>> predicate, CancellationToken cancellationToken = default);
		public abstract Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default);
		public abstract Task AddRangeAsync(IEnumerable<TAggregate> entities, CancellationToken cancellationToken = default);
		public abstract void Update(TAggregate entity);
		public abstract void Remove(TAggregate entity);
	}
}


