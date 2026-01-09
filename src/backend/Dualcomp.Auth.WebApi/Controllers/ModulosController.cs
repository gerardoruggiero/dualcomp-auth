using Dualcomp.Auth.Application.Modulos.GetModulos;
using Dualcomp.Auth.Application.Modulos.CreateModulo;
using Dualcomp.Auth.Application.Modulos.UpdateModulo;
using Dualcomp.Auth.Application.Modulos.ActivateModulo;
using Dualcomp.Auth.Application.Modulos.DeactivateModulo;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ModulosController : BaseTypesController<
		GetModulosQuery, 
		GetModulosResult,
		CreateModuloCommand,
		CreateModuloResult,
		UpdateModuloCommand,
		UpdateModuloResult,
		ActivateModuloCommand,
		DeactivateModuloCommand>
	{
		public ModulosController(
			IQueryHandler<GetModulosQuery, GetModulosResult> getModulosHandler,
			ICommandHandler<CreateModuloCommand, CreateModuloResult> createModuloHandler,
			ICommandHandler<UpdateModuloCommand, UpdateModuloResult> updateModuloHandler,
			ICommandHandler<ActivateModuloCommand> activateModuloHandler,
			ICommandHandler<DeactivateModuloCommand> deactivateModuloHandler) 
			: base(getModulosHandler, createModuloHandler, updateModuloHandler, activateModuloHandler, deactivateModuloHandler)
		{
		}
	}
}
