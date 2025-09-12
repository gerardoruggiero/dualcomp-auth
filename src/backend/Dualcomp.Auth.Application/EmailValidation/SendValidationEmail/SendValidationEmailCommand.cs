using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.EmailValidation.SendValidationEmail
{
    public class SendValidationEmailCommand : ICommand<SendValidationEmailResult>
    {
        public Guid UserId { get; set; }

        public SendValidationEmailCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
