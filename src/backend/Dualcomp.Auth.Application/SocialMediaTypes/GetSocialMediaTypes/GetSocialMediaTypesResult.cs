namespace Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes
{
	public record SocialMediaTypeItem(string Value);

	public record GetSocialMediaTypesResult(IEnumerable<SocialMediaTypeItem> SocialMediaTypes);
}
