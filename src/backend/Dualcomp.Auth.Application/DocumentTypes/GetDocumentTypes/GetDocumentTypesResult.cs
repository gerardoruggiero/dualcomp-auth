namespace Dualcomp.Auth.Application.DocumentTypes.GetDocumentTypes
{
	public record DocumentTypeItem(string Id, string Name, string? Description, bool IsActive);

	public record GetDocumentTypesResult(IEnumerable<DocumentTypeItem> DocumentTypes);
}

