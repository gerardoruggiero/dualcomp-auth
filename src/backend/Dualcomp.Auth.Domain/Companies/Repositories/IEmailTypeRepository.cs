using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Domain.Companies.Repositories
{
	public interface IEmailTypeRepository : IRepository<EmailTypeEntity>
	{
		Task<IEnumerable<EmailTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	}
}