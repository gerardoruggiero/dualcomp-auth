using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes
{
	public record GetPhoneTypesQuery() : IQuery<GetPhoneTypesResult>;
}
