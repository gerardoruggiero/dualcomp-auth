using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;

namespace Dualcomp.Auth.Application.Users.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordGenerator passwordGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        // Buscar usuario por email
        var email = Email.Create(command.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        
        if (user == null)
        {
            return new ResetPasswordResult
            {
                Success = false,
                Message = "Usuario no encontrado"
            };
        }

        // Generar nueva contraseña temporal
        var temporaryPassword = _passwordGenerator.GenerateTemporaryPassword();
        var hashedPassword = HashedPassword.Create(_passwordHasher.HashPassword(temporaryPassword));

        // Actualizar contraseña del usuario
        user.UpdatePassword(hashedPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Enviar la contraseña temporal por email
        // Por ahora, la retornamos en el resultado (solo para desarrollo/testing)

        return new ResetPasswordResult
        {
            Success = true,
            TemporaryPassword = temporaryPassword, // Solo para desarrollo - en producción no retornar
            Message = "Contraseña restablecida exitosamente. Se ha enviado una contraseña temporal por email."
        };
    }
}
