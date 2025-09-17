namespace Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes
{
	public record SocialMediaTypeItem(string Id, string Name, string? Description, bool IsActive);

	public record GetSocialMediaTypesResult(IEnumerable<SocialMediaTypeItem> SocialMediaTypes);
}
