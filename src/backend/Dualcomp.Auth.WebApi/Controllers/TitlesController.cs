using Dualcomp.Auth.Application.Titles.GetTitles;
using Dualcomp.Auth.Application.Titles.CreateTitle;
using Dualcomp.Auth.Application.Titles.UpdateTitle;
using Dualcomp.Auth.Application.Titles.ActivateTitle;
using Dualcomp.Auth.Application.Titles.DeactivateTitle;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TitlesController : BaseTypesController<
		GetTitlesQuery, 
		GetTitlesResult,
		CreateTitleCommand,
		CreateTitleResult,
		UpdateTitleCommand,
		UpdateTitleResult,
		ActivateTitleCommand,
		DeactivateTitleCommand>
	{
        public TitlesController(
			IQueryHandler<GetTitlesQuery, GetTitlesResult> getTitlesHandler,
			ICommandHandler<CreateTitleCommand, CreateTitleResult> createTitleHandler,
			ICommandHandler<UpdateTitleCommand, UpdateTitleResult> updateTitleHandler,
			ICommandHandler<ActivateTitleCommand> activateTitleHandler,
			ICommandHandler<DeactivateTitleCommand> deactivateTitleHandler) 
            : base(getTitlesHandler, createTitleHandler, updateTitleHandler, activateTitleHandler, deactivateTitleHandler)
        {
        }
	}
}

