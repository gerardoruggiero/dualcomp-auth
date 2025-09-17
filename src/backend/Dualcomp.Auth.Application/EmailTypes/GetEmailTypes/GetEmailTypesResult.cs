namespace Dualcomp.Auth.Application.EmailTypes.GetEmailTypes
{
	public record EmailTypeItem(string Id, string Name, string? Description, bool IsActive);

	public record GetEmailTypesResult(IEnumerable<EmailTypeItem> EmailTypes);
}
