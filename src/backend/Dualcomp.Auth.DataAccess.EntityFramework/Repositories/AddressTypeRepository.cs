using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.DataAccess.EntityFramework;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.DataAccess.EntityFramework.Repositories
{
	public class AddressTypeRepository : EfRepository<AddressTypeEntity>, IAddressTypeRepository
	{
        public AddressTypeRepository(BaseDbContext dbContext) : base(dbContext)
        {
        }
	}
}
