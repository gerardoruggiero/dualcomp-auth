namespace Dualcomp.Auth.Domain.Companies
{
	public class SocialMediaTypeEntity : BaseTypeEntity
	{
		private SocialMediaTypeEntity() { }

		private SocialMediaTypeEntity(string name, string? description = null) : base(name, description)
		{
		}

		public static SocialMediaTypeEntity Create(string name, string? description = null)
			=> new SocialMediaTypeEntity(name, description);
	}
}
