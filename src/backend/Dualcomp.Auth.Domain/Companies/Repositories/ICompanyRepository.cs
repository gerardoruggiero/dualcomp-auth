using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.Domain.Companies.Repositories
{
	public interface ICompanyRepository : IRepository<Company>
	{
		Task<bool> ExistsByTaxIdAsync(string normalizedTaxId, CancellationToken cancellationToken = default);
		Task<bool> ExistsByTaxIdForOtherCompanyAsync(string normalizedTaxId, Guid excludeCompanyId, CancellationToken cancellationToken = default);
		Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<Company?> GetByTaxIdAsync(TaxId taxId, CancellationToken cancellationToken = default);
		Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default);
		Task UpdateAsync(Company company, CancellationToken cancellationToken = default);
		Task<CompanyWithTypes?> GetByIdWithTypesAsync(Guid id, CancellationToken cancellationToken = default);
	}

	/// <summary>
	/// Contenedor para una empresa con todos sus tipos de contacto cargados
	/// </summary>
	public record CompanyWithTypes(
		Company Company,
		Dictionary<Guid, string> AddressTypeNames,
		Dictionary<Guid, string> EmailTypeNames,
		Dictionary<Guid, string> PhoneTypeNames,
		Dictionary<Guid, string> SocialMediaTypeNames
	);
}
