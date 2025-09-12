namespace Dualcomp.Auth.Application.AddressTypes.GetAddressTypes
{
	public record AddressTypeItem(string Id, string Value);

	public record GetAddressTypesResult(IEnumerable<AddressTypeItem> AddressTypes);
}
