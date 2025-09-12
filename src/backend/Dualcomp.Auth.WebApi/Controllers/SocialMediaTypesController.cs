using Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SocialMediaTypesController : ControllerBase
	{
		private readonly IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult> _getSocialMediaTypesHandler;

        public SocialMediaTypesController(IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult> getSocialMediaTypesHandler) => _getSocialMediaTypesHandler = getSocialMediaTypesHandler ?? throw new ArgumentNullException(nameof(getSocialMediaTypesHandler));

        [HttpGet]
		public async Task<IActionResult> GetSocialMediaTypes(CancellationToken cancellationToken)
		{
			try
			{
				var query = new GetSocialMediaTypesQuery();
				var result = await _getSocialMediaTypesHandler.Handle(query, cancellationToken);

				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}
	}
}
