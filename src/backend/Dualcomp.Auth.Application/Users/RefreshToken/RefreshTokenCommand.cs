using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.RefreshToken
{
    public record RefreshTokenCommand(
        string RefreshToken,
        string? UserAgent = null,
        string? IpAddress = null
    ) : ICommand<RefreshTokenResult>;
}
