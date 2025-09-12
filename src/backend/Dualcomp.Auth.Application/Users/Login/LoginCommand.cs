using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.Application.Users.Login
{
    public record LoginCommand(
        Email Email,
        string Password,
        string? UserAgent = null,
        string? IpAddress = null
    ) : ICommand<LoginResult>;
}
