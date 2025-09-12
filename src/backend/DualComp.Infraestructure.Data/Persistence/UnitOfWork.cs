namespace DualComp.Infraestructure.Data.Persistence
{
	public abstract class UnitOfWork : IUnitOfWork
	{
		public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}


