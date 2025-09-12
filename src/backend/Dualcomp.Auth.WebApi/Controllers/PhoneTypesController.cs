using Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PhoneTypesController : ControllerBase
	{
		private readonly IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult> _getPhoneTypesHandler;

        public PhoneTypesController(IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult> getPhoneTypesHandler) => _getPhoneTypesHandler = getPhoneTypesHandler ?? throw new ArgumentNullException(nameof(getPhoneTypesHandler));

        [HttpGet]
		public async Task<IActionResult> GetPhoneTypes(CancellationToken cancellationToken)
		{
			try
			{
				var query = new GetPhoneTypesQuery();
				var result = await _getPhoneTypesHandler.Handle(query, cancellationToken);

				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}
	}
}
