using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Domain.Companies.Repositories
{
	public interface IModuloRepository : IRepository<ModuloEntity>
	{
		Task<IEnumerable<ModuloEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	}
}
