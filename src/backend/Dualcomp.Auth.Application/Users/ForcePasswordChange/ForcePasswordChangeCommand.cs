using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.ForcePasswordChange
{
    public class ForcePasswordChangeCommand : ICommand<ForcePasswordChangeResult>
    {
        public Guid UserId { get; set; }
        public string TemporaryPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public ForcePasswordChangeCommand(Guid userId, string temporaryPassword, string newPassword)
        {
            UserId = userId;
            TemporaryPassword = temporaryPassword;
            NewPassword = newPassword;
        }
    }
}

