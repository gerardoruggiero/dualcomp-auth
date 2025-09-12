using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.Logout
{
    public record LogoutCommand(
        string AccessToken
    ) : ICommand<LogoutResult>;
}
