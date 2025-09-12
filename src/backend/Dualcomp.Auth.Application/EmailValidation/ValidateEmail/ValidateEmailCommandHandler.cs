using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users;

namespace Dualcomp.Auth.Application.EmailValidation.ValidateEmail
{
    public class ValidateEmailCommandHandler : ICommandHandler<ValidateEmailCommand, ValidateEmailResult>
    {
        private readonly IEmailValidationRepository _emailValidationRepository;
        private readonly IUserRepository _userRepository;

        public ValidateEmailCommandHandler(
            IEmailValidationRepository emailValidationRepository,
            IUserRepository userRepository)
        {
            _emailValidationRepository = emailValidationRepository;
            _userRepository = userRepository;
        }

        public async Task<ValidateEmailResult> Handle(ValidateEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar el token de validación
                var emailValidation = await _emailValidationRepository.GetByTokenAsync(request.Token, cancellationToken);
                
                if (emailValidation == null)
                {
                    return ValidateEmailResult.Failure("Token de validación no encontrado");
                }

                // Verificar si el token ya fue usado
                if (emailValidation.IsUsed)
                {
                    return ValidateEmailResult.Failure("El token de validación ya ha sido utilizado");
                }

                // Verificar si el token ha expirado
                if (emailValidation.IsExpired())
                {
                    return ValidateEmailResult.Failure("El token de validación ha expirado");
                }

                // Buscar el usuario
                var user = await _userRepository.GetByIdAsync(emailValidation.UserId, cancellationToken);
                
                if (user == null)
                {
                    return ValidateEmailResult.Failure("Usuario no encontrado");
                }

                // Verificar si el email ya fue validado
                if (user.IsEmailValidated)
                {
                    return ValidateEmailResult.Failure("El email ya ha sido validado anteriormente");
                }

                // Validar el email del usuario
                user.ValidateEmail();

                // Marcar el token como usado
                emailValidation.MarkAsUsed();

                // Actualizar en la base de datos
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _emailValidationRepository.UpdateAsync(emailValidation, cancellationToken);

                return ValidateEmailResult.Success(user.Id, user.Email.Value);
            }
            catch (Exception ex)
            {
                return ValidateEmailResult.Failure($"Error al validar el email: {ex.Message}");
            }
        }
    }
}
