using Dualcomp.Auth.Application.AddressTypes.GetAddressTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AddressTypesController : ControllerBase
	{
		private readonly IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult> _getAddressTypesHandler;

        public AddressTypesController(IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult> getAddressTypesHandler) => _getAddressTypesHandler = getAddressTypesHandler ?? throw new ArgumentNullException(nameof(getAddressTypesHandler));

        [HttpGet]
		public async Task<IActionResult> GetAddressTypes(CancellationToken cancellationToken)
		{
			try
			{
				var query = new GetAddressTypesQuery();
				var result = await _getAddressTypesHandler.Handle(query, cancellationToken);

				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}
	}
}
