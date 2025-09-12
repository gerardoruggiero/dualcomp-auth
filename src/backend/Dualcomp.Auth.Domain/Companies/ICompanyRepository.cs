using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.Domain.Companies
{
	public interface ICompanyRepository : IRepository<Company>
	{
		Task<bool> ExistsByTaxIdAsync(string normalizedTaxId, CancellationToken cancellationToken = default);
		Task<Company?> GetByTaxIdAsync(TaxId taxId, CancellationToken cancellationToken = default);
		Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default);
		Task UpdateAsync(Company company, CancellationToken cancellationToken = default);
	}
}
