namespace Dualcomp.Auth.Domain.Companies
{
	public class AddressTypeEntity : BaseTypeEntity
	{
		private AddressTypeEntity() { }

		private AddressTypeEntity(string name, string? description = null) : base(name, description)
		{
		}

		public static AddressTypeEntity Create(string name, string? description = null)
			=> new AddressTypeEntity(name, description);
	}
}
