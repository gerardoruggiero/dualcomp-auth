using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.DocumentTypes.GetDocumentTypes
{
	public record GetDocumentTypesQuery() : IQuery<GetDocumentTypesResult>;
}

