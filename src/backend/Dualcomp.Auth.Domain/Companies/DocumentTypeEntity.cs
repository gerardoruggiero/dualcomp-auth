namespace Dualcomp.Auth.Domain.Companies
{
	public class DocumentTypeEntity : BaseTypeEntity
	{
		private DocumentTypeEntity() { }

		private DocumentTypeEntity(string name, string? description = null) : base(name, description)
		{
		}

		public static DocumentTypeEntity Create(string name, string? description = null)
			=> new DocumentTypeEntity(name, description);
	}
}

