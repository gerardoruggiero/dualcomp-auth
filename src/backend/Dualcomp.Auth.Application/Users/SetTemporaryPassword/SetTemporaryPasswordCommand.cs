using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.SetTemporaryPassword
{
    public class SetTemporaryPasswordCommand : ICommand<SetTemporaryPasswordResult>
    {
        public Guid TargetUserId { get; set; }
        public Guid AdminUserId { get; set; }
        public string TemporaryPassword { get; set; } = string.Empty;

        public SetTemporaryPasswordCommand(Guid targetUserId, Guid adminUserId, string temporaryPassword)
        {
            TargetUserId = targetUserId;
            AdminUserId = adminUserId;
            TemporaryPassword = temporaryPassword;
        }
    }
}

