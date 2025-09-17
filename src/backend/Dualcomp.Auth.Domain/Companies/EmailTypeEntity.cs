namespace Dualcomp.Auth.Domain.Companies
{
	public class EmailTypeEntity : BaseTypeEntity
	{
		private EmailTypeEntity() { }

		private EmailTypeEntity(string name, string? description = null) : base(name, description)
		{
		}

		public static EmailTypeEntity Create(string name, string? description = null)
			=> new EmailTypeEntity(name, description);
	}
}
