namespace Dualcomp.Auth.Domain.Companies.Repositories
{
    public interface ICompanySettingsRepository
    {
        Task<CompanySettings?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<CompanySettings?> GetActiveByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CompanySettings>> GetActiveSettingsAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task AddAsync(CompanySettings companySettings, CancellationToken cancellationToken = default);
        Task UpdateAsync(CompanySettings companySettings, CancellationToken cancellationToken = default);
        Task DeleteAsync(CompanySettings companySettings, CancellationToken cancellationToken = default);
        Task DeactivateByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    }
}
