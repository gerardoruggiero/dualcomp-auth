using Dualcomp.Auth.Application.DocumentTypes.GetDocumentTypes;
using Dualcomp.Auth.Application.DocumentTypes.CreateDocumentType;
using Dualcomp.Auth.Application.DocumentTypes.UpdateDocumentType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DocumentTypesController : BaseTypesController<
		GetDocumentTypesQuery, 
		GetDocumentTypesResult,
		CreateDocumentTypeCommand,
		CreateDocumentTypeResult,
		UpdateDocumentTypeCommand,
		UpdateDocumentTypeResult>
	{
        public DocumentTypesController(
			IQueryHandler<GetDocumentTypesQuery, GetDocumentTypesResult> getDocumentTypesHandler,
			ICommandHandler<CreateDocumentTypeCommand, CreateDocumentTypeResult> createDocumentTypeHandler,
			ICommandHandler<UpdateDocumentTypeCommand, UpdateDocumentTypeResult> updateDocumentTypeHandler) 
            : base(getDocumentTypesHandler, createDocumentTypeHandler, updateDocumentTypeHandler)
        {
        }
	}
}

