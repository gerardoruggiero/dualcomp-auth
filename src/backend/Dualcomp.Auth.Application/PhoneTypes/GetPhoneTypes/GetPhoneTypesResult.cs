namespace Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes
{
	public record PhoneTypeItem(string Id, string Name, string? Description, bool IsActive);

	public record GetPhoneTypesResult(IEnumerable<PhoneTypeItem> PhoneTypes);
}
