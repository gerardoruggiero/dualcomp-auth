using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.EmailTypes.GetEmailTypes
{
	public record GetEmailTypesQuery() : IQuery<GetEmailTypesResult>;
}
