namespace Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes
{
	public record PhoneTypeItem(string Value);

	public record GetPhoneTypesResult(IEnumerable<PhoneTypeItem> PhoneTypes);
}
