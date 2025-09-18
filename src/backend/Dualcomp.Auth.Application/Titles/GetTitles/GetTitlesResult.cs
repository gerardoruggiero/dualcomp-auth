namespace Dualcomp.Auth.Application.Titles.GetTitles
{
	public record TitleItem(string Id, string Name, string? Description, bool IsActive);

	public record GetTitlesResult(IEnumerable<TitleItem> Titles);
}
