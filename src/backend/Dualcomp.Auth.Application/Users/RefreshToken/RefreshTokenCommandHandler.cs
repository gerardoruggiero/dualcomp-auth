using Dualcomp.Auth.Application.Abstractions.Messaging;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.Application.Users.RefreshToken
{
    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository,
            IJwtTokenService jwtTokenService,
            JwtSettings jwtSettings,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Buscar sesión por refresh token
            var session = await _userSessionRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
            
            if (session == null || !session.IsActive || session.IsExpired())
            {
                throw new UnauthorizedAccessException("Token de actualización inválido o expirado");
            }

            // Buscar usuario
            var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
            if (user == null || !user.IsActive)
            {
                throw new InvalidOperationException("Usuario no encontrado o inactivo");
            }

            // Generar nuevos tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(
                user.Id,
                user.Email.Value,
                user.CompanyId,
                session.Id,
                user.IsCompanyAdmin);

            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            var newExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            // Actualizar sesión con nuevos tokens
            session.UpdateTokens(newAccessToken, newRefreshToken, newExpiresAt);
            await _userSessionRepository.UpdateAsync(session, cancellationToken);

            // Persistir los cambios en la base de datos
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RefreshTokenResult(
                newAccessToken,
                newRefreshToken,
                newExpiresAt,
                user.Id,
                user.Email.Value,
                user.GetFullName(),
                user.CompanyId,
                user.IsCompanyAdmin);
        }
    }
}
