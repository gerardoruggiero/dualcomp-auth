using System.Linq.Expressions;

namespace DualComp.Infraestructure.Data.Persistence
{
	public interface IRepository<TAggregate>
		where TAggregate : class
	{
		Task<TAggregate?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<TAggregate>> ListAsync(CancellationToken cancellationToken = default);
		Task<IReadOnlyList<TAggregate>> ListAsync(Expression<Func<TAggregate, bool>> predicate, CancellationToken cancellationToken = default);
		Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default);
		Task AddRangeAsync(IEnumerable<TAggregate> entities, CancellationToken cancellationToken = default);
		Task<TAggregate> UpdateAsync(TAggregate item, CancellationToken cancellationToken = default);
        void Update(TAggregate entity);
		void Remove(TAggregate entity);
	}
}


