using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.ResetUserPassword
{
    public record ResetUserPasswordCommand(
        Guid TargetUserId,
        Guid AdminUserId
    ) : ICommand<ResetUserPasswordResult>;

    public record ResetUserPasswordResult(
        Guid UserId,
        string Email,
        string FullName,
        string TemporaryPassword,
        bool IsSuccess,
        string Message
    )
    {
        public static ResetUserPasswordResult Success(Guid userId, string email, string fullName, string temporaryPassword, string message)
        {
            return new ResetUserPasswordResult(userId, email, fullName, temporaryPassword, true, message);
        }

        public static ResetUserPasswordResult Failure(string message)
        {
            return new ResetUserPasswordResult(Guid.Empty, string.Empty, string.Empty, string.Empty, false, message);
        }
    }
}
