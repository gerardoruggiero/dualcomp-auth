using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes
{
	public record GetSocialMediaTypesQuery() : IQuery<GetSocialMediaTypesResult>;
}
