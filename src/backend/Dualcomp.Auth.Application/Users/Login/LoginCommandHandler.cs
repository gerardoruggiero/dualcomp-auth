using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.Application.Users.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            JwtSettings jwtSettings,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Verificar si el email está validado
            if (!user.IsEmailValidated)
            {
                throw new UnauthorizedAccessException("Debes validar tu email antes de iniciar sesión. Revisa tu bandeja de entrada.");
            }

            // Verificar contraseña
            if (!_passwordHasher.VerifyPassword(request.Password, user.Password.Value))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Verificar si requiere cambio obligatorio de contraseña
            if (user.RequiresPasswordChange())
            {
                // Si requiere cambio de contraseña, no generar tokens completos
                // Solo retornar información básica para redirigir al cambio
                return new LoginResult(
                    string.Empty, // No access token
                    string.Empty, // No refresh token
                    DateTime.MinValue, // No expiration
                    user.Id,
                    user.Email.Value,
                    user.GetFullName(),
                    user.CompanyId,
                    user.IsCompanyAdmin,
                    RequiresPasswordChange: true,
                    IsEmailValidated: user.IsEmailValidated);
            }

            // Invalidar sesiones previas del usuario
            await _userSessionRepository.DeactivateAllUserSessionsAsync(user.Id, cancellationToken);

            // Generar sessionId único para esta sesión
            var sessionId = Guid.NewGuid();
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            // Crear nueva sesión primero
            var session = UserSession.Create(
                user.Id,
                string.Empty, // accessToken se actualizará después
                refreshToken,
                expiresAt,
                request.UserAgent,
                request.IpAddress);

            // Generar access token con el sessionId correcto
            var accessToken = _jwtTokenService.GenerateAccessToken(
                user.Id, 
                user.Email.Value, 
                user.CompanyId, 
                sessionId,
                user.IsCompanyAdmin);

            // Actualizar la sesión con el access token
            session.UpdateTokens(accessToken, refreshToken, expiresAt);

            await _userSessionRepository.AddAsync(session, cancellationToken);

            // Actualizar último login del usuario
            user.SetLastLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Persistir todos los cambios en la base de datos
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResult(
                accessToken,
                refreshToken,
                expiresAt,
                user.Id,
                user.Email.Value,
                user.GetFullName(),
                user.CompanyId,
                user.IsCompanyAdmin,
                RequiresPasswordChange: false,
                IsEmailValidated: user.IsEmailValidated);
        }
    }
}
