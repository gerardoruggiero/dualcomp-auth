using Dualcomp.Auth.Application.Modulos.GetModulos;
using Dualcomp.Auth.Application.Modulos.CreateModulo;
using Dualcomp.Auth.Application.Modulos.UpdateModulo;
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
		UpdateModuloResult>
	{
		public ModulosController(
			IQueryHandler<GetModulosQuery, GetModulosResult> getModulosHandler,
			ICommandHandler<CreateModuloCommand, CreateModuloResult> createModuloHandler,
			ICommandHandler<UpdateModuloCommand, UpdateModuloResult> updateModuloHandler) 
			: base(getModulosHandler, createModuloHandler, updateModuloHandler)
		{
		}
	}
}
