namespace Dualcomp.Auth.Application.AddressTypes.GetAddressTypes
{
	public record AddressTypeItem(string Value);

	public record GetAddressTypesResult(IEnumerable<AddressTypeItem> AddressTypes);
}
