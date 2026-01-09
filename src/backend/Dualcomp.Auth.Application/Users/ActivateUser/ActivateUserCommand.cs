using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.ActivateUser
{
    public record ActivateUserCommand(
        Guid UserId,
        Guid ActivatedBy
    ) : ICommand<ActivateUserResult>;

    public record ActivateUserResult(
        Guid UserId,
        bool IsActive
    );
}
