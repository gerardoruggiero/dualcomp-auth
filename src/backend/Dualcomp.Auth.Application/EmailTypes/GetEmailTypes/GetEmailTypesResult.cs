namespace Dualcomp.Auth.Application.EmailTypes.GetEmailTypes
{
	public record EmailTypeItem(string Id, string Value);

	public record GetEmailTypesResult(IEnumerable<EmailTypeItem> EmailTypes);
}
