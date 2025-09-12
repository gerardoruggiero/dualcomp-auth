using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.EmailValidation.ValidateEmail
{
    public class ValidateEmailCommand : ICommand<ValidateEmailResult>
    {
        public string Token { get; set; } = string.Empty;

        public ValidateEmailCommand(string token)
        {
            Token = token;
        }
    }
}
