using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Domain.Companies.Repositories
{
	public interface IDocumentTypeRepository : IRepository<DocumentTypeEntity>
	{
		Task<IEnumerable<DocumentTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	}
}

