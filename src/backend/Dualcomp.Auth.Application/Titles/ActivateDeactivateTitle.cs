using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Titles.ActivateTitle
{
    public class ActivateTitleCommand : ActivateTypeCommandBase { }
    public class ActivateTitleCommandHandler : ICommandHandler<ActivateTitleCommand>
    {
        private readonly ITitleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public ActivateTitleCommandHandler(ITitleRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(ActivateTitleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"Title with ID {request.Id} not found");
            entity.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

namespace Dualcomp.Auth.Application.Titles.DeactivateTitle
{
    public class DeactivateTitleCommand : DeactivateTypeCommandBase { }
    public class DeactivateTitleCommandHandler : ICommandHandler<DeactivateTitleCommand>
    {
        private readonly ITitleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public DeactivateTitleCommandHandler(ITitleRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(DeactivateTitleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"Title with ID {request.Id} not found");
            entity.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
