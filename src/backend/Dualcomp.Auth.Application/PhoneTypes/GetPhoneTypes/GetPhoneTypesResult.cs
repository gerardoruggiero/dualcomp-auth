namespace Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes
{
	public record PhoneTypeItem(string Id, string Value);

	public record GetPhoneTypesResult(IEnumerable<PhoneTypeItem> PhoneTypes);
}
