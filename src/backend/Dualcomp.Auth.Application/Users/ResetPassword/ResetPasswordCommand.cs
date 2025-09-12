using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.ResetPassword;

public class ResetPasswordCommand : ICommand<ResetPasswordResult>
{
    public string Email { get; init; } = string.Empty;
}

public class ResetPasswordResult
{
    public bool Success { get; init; }
    public string? TemporaryPassword { get; init; }
    public string Message { get; init; } = string.Empty;
}
