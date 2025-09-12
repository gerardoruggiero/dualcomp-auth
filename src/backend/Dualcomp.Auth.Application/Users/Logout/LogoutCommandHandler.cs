using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users;
using DualComp.Infraestructure.Domain.Domain.Common.Results;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Users.Logout
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand, LogoutResult>
    {
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IUserSessionRepository userSessionRepository, IUnitOfWork unitOfWork)
        {
            _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<LogoutResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Buscar sesión por access token
            var session = await _userSessionRepository.GetByAccessTokenAsync(request.AccessToken, cancellationToken);
            
            if (session == null || !session.IsActive)
            {
                return new LogoutResult(
                    true, 
                    "Sesión ya cerrada o no encontrada");
            }

            // Eliminar completamente la sesión de la base de datos
            await _userSessionRepository.DeleteAsync(session, cancellationToken);

            // Persistir los cambios en la base de datos
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LogoutResult(
                true, 
                "Sesión cerrada exitosamente");
        }
    }
}
