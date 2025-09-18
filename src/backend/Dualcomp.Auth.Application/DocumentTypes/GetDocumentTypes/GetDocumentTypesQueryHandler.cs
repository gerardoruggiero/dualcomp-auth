using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.Application.DocumentTypes.GetDocumentTypes
{
	public class GetDocumentTypesQueryHandler : GetTypesQueryHandler<DocumentTypeEntity, IDocumentTypeRepository, GetDocumentTypesQuery, GetDocumentTypesResult>
	{
        public GetDocumentTypesQueryHandler(IDocumentTypeRepository documentTypeRepository) 
            : base(
                documentTypeRepository,
                (repo, ct) => repo.GetAllAsync(ct),
                entities => new GetDocumentTypesResult(entities.Select(e => new DocumentTypeItem(e.Id.ToString(), e.Name, e.Description, e.IsActive))))
        {
        }
	}
}

