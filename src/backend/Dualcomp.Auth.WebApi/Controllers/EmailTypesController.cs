using Dualcomp.Auth.Application.EmailTypes.GetEmailTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EmailTypesController : ControllerBase
	{
		private readonly IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult> _getEmailTypesHandler;

        public EmailTypesController(IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult> getEmailTypesHandler) => _getEmailTypesHandler = getEmailTypesHandler ?? throw new ArgumentNullException(nameof(getEmailTypesHandler));

        [HttpGet]
		public async Task<IActionResult> GetEmailTypes(CancellationToken cancellationToken)
		{
			try
			{
				var query = new GetEmailTypesQuery();
				var result = await _getEmailTypesHandler.Handle(query, cancellationToken);

				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}
	}
}
