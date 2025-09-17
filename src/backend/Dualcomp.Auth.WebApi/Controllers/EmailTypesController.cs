using Dualcomp.Auth.Application.EmailTypes.GetEmailTypes;
using Dualcomp.Auth.Application.EmailTypes.CreateEmailType;
using Dualcomp.Auth.Application.EmailTypes.UpdateEmailType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EmailTypesController : BaseTypesController<
		GetEmailTypesQuery, 
		GetEmailTypesResult,
		CreateEmailTypeCommand,
		CreateEmailTypeResult,
		UpdateEmailTypeCommand,
		UpdateEmailTypeResult>
	{
        public EmailTypesController(
			IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult> getEmailTypesHandler,
			ICommandHandler<CreateEmailTypeCommand, CreateEmailTypeResult> createEmailTypeHandler,
			ICommandHandler<UpdateEmailTypeCommand, UpdateEmailTypeResult> updateEmailTypeHandler) 
            : base(getEmailTypesHandler, createEmailTypeHandler, updateEmailTypeHandler)
        {
        }
	}
}
