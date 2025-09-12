using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.AddressTypes.GetAddressTypes
{
	public record GetAddressTypesQuery() : IQuery<GetAddressTypesResult>;
}
