namespace Dualcomp.Auth.Domain.Companies
{
	public class PhoneTypeEntity : BaseTypeEntity
	{
		private PhoneTypeEntity() { }

		private PhoneTypeEntity(string name, string? description = null) : base(name, description)
		{
		}

		public static PhoneTypeEntity Create(string name, string? description = null)
			=> new PhoneTypeEntity(name, description);
	}
}
