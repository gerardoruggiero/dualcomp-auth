using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.Application.Users.ChangePassword
{
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, ChangePasswordResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository,
            IPasswordHasher passwordHasher,
            IPasswordValidator passwordValidator,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _passwordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ChangePasswordResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Buscar usuario
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null || !user.IsActive)
            {
                throw new InvalidOperationException("Usuario no encontrado o inactivo");
            }

            // Verificar contraseña actual
            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.Password.Value))
            {
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta");
            }

            // Validar nueva contraseña
            if (!_passwordValidator.IsValid(request.NewPassword, out string validationError))
            {
                throw new ArgumentException(validationError);
            }

            // Verificar que la nueva contraseña sea diferente a la actual
            if (_passwordHasher.VerifyPassword(request.NewPassword, user.Password.Value))
            {
                throw new ArgumentException("La nueva contraseña debe ser diferente a la actual");
            }

            // Hash de la nueva contraseña
            var hashedNewPassword = HashedPassword.Create(_passwordHasher.HashPassword(request.NewPassword));

            // Actualizar contraseña
            user.UpdatePassword(hashedNewPassword);
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Invalidar todas las sesiones del usuario (forzar re-login)
            await _userSessionRepository.DeactivateAllUserSessionsAsync(user.Id, cancellationToken);

            // Persistir todos los cambios en la base de datos
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ChangePasswordResult(
                true,
                "Contraseña actualizada exitosamente. Debe iniciar sesión nuevamente.");
        }
    }
}
