using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Domain.Companies.Repositories
{
	public interface ITitleRepository : IRepository<TitleEntity>
	{
		Task<IEnumerable<TitleEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	}
}

