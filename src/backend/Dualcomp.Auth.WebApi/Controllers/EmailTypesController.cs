using Dualcomp.Auth.Application.EmailTypes.GetEmailTypes;
using Dualcomp.Auth.Application.EmailTypes.CreateEmailType;
using Dualcomp.Auth.Application.EmailTypes.UpdateEmailType;
using Dualcomp.Auth.Application.EmailTypes.ActivateEmailType;
using Dualcomp.Auth.Application.EmailTypes.DeactivateEmailType;
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
		UpdateEmailTypeResult,
		ActivateEmailTypeCommand,
		DeactivateEmailTypeCommand>
	{
        public EmailTypesController(
			IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult> getEmailTypesHandler,
			ICommandHandler<CreateEmailTypeCommand, CreateEmailTypeResult> createEmailTypeHandler,
			ICommandHandler<UpdateEmailTypeCommand, UpdateEmailTypeResult> updateEmailTypeHandler,
			ICommandHandler<ActivateEmailTypeCommand> activateEmailTypeHandler,
			ICommandHandler<DeactivateEmailTypeCommand> deactivateEmailTypeHandler) 
            : base(getEmailTypesHandler, createEmailTypeHandler, updateEmailTypeHandler, activateEmailTypeHandler, deactivateEmailTypeHandler)
        {
        }
	}
}
