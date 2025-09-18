namespace Dualcomp.Auth.Domain.Companies
{
	public class TitleEntity : BaseTypeEntity
	{
		private TitleEntity() { }

		private TitleEntity(string name, string? description = null) : base(name, description)
		{
		}

		public static TitleEntity Create(string name, string? description = null)
			=> new TitleEntity(name, description);
	}
}

