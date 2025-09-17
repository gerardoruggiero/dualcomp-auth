using Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes;
using Dualcomp.Auth.Application.SocialMediaTypes.CreateSocialMediaType;
using Dualcomp.Auth.Application.SocialMediaTypes.UpdateSocialMediaType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SocialMediaTypesController : BaseTypesController<
		GetSocialMediaTypesQuery, 
		GetSocialMediaTypesResult,
		CreateSocialMediaTypeCommand,
		CreateSocialMediaTypeResult,
		UpdateSocialMediaTypeCommand,
		UpdateSocialMediaTypeResult>
	{
        public SocialMediaTypesController(
			IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult> getSocialMediaTypesHandler,
			ICommandHandler<CreateSocialMediaTypeCommand, CreateSocialMediaTypeResult> createSocialMediaTypeHandler,
			ICommandHandler<UpdateSocialMediaTypeCommand, UpdateSocialMediaTypeResult> updateSocialMediaTypeHandler) 
            : base(getSocialMediaTypesHandler, createSocialMediaTypeHandler, updateSocialMediaTypeHandler)
        {
        }
	}
}
