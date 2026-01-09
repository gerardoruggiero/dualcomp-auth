using Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes;
using Dualcomp.Auth.Application.PhoneTypes.CreatePhoneType;
using Dualcomp.Auth.Application.PhoneTypes.UpdatePhoneType;
using Dualcomp.Auth.Application.PhoneTypes.ActivatePhoneType;
using Dualcomp.Auth.Application.PhoneTypes.DeactivatePhoneType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PhoneTypesController : BaseTypesController<
		GetPhoneTypesQuery, 
		GetPhoneTypesResult,
		CreatePhoneTypeCommand,
		CreatePhoneTypeResult,
		UpdatePhoneTypeCommand,
		UpdatePhoneTypeResult,
		ActivatePhoneTypeCommand,
		DeactivatePhoneTypeCommand>
	{
        public PhoneTypesController(
			IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult> getPhoneTypesHandler,
			ICommandHandler<CreatePhoneTypeCommand, CreatePhoneTypeResult> createPhoneTypeHandler,
			ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult> updatePhoneTypeHandler,
			ICommandHandler<ActivatePhoneTypeCommand> activatePhoneTypeHandler,
			ICommandHandler<DeactivatePhoneTypeCommand> deactivatePhoneTypeHandler) 
            : base(getPhoneTypesHandler, createPhoneTypeHandler, updatePhoneTypeHandler, activatePhoneTypeHandler, deactivatePhoneTypeHandler)
        {
        }
	}
}
