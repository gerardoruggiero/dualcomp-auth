using Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SocialMediaTypesController : BaseTypesController<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>
	{
        public SocialMediaTypesController(IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult> getSocialMediaTypesHandler) 
            : base(getSocialMediaTypesHandler)
        {
        }
	}
}
