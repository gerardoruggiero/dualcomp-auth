using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.DeactivateUser
{
    public record DeactivateUserCommand(
        Guid UserId,
        Guid DeactivatedBy
    ) : ICommand<DeactivateUserResult>;

    public record DeactivateUserResult(
        Guid UserId,
        bool IsActive
    );
}
