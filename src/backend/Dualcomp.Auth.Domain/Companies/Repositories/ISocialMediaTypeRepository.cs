using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Domain.Companies.Repositories
{
	public interface ISocialMediaTypeRepository : IRepository<SocialMediaTypeEntity>
	{
		Task<IEnumerable<SocialMediaTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	}
}