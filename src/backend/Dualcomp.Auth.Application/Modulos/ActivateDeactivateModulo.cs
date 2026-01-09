using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Modulos.ActivateModulo
{
    public class ActivateModuloCommand : ActivateTypeCommandBase { }
    public class ActivateModuloCommandHandler : ICommandHandler<ActivateModuloCommand>
    {
        private readonly IModuloRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public ActivateModuloCommandHandler(IModuloRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(ActivateModuloCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"Modulo with ID {request.Id} not found");
            entity.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

namespace Dualcomp.Auth.Application.Modulos.DeactivateModulo
{
    public class DeactivateModuloCommand : DeactivateTypeCommandBase { }
    public class DeactivateModuloCommandHandler : ICommandHandler<DeactivateModuloCommand>
    {
        private readonly IModuloRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public DeactivateModuloCommandHandler(IModuloRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(DeactivateModuloCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"Modulo with ID {request.Id} not found");
            entity.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
