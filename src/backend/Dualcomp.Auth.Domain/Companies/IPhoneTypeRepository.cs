using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Domain.Companies
{
	public interface IPhoneTypeRepository : IRepository<PhoneTypeEntity>
	{
		Task<IEnumerable<PhoneTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	}
}