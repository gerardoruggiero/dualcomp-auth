using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Domain.Companies
{
	public interface IAddressTypeRepository : IRepository<AddressTypeEntity>
	{
		Task<IEnumerable<AddressTypeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	}
}