namespace Dualcomp.Auth.Domain.Companies
{
	public class ModuloEntity : BaseTypeEntity
	{
		private ModuloEntity() { }

		private ModuloEntity(string name, string? description = null) : base(name, description)
		{
		}

		public static ModuloEntity Create(string name, string? description = null)
			=> new ModuloEntity(name, description);
	}
}
