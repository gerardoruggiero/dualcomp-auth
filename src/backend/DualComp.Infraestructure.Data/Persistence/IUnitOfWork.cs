namespace DualComp.Infraestructure.Data.Persistence
{
	public interface IUnitOfWork
	{
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}


