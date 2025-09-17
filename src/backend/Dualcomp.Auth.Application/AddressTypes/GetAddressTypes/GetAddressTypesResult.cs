namespace Dualcomp.Auth.Application.AddressTypes.GetAddressTypes
{
	public record AddressTypeItem(string Id, string Name, string? Description, bool IsActive);

	public record GetAddressTypesResult(IEnumerable<AddressTypeItem> AddressTypes);
}
