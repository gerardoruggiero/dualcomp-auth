using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.DocumentTypes.ActivateDocumentType
{
    public class ActivateDocumentTypeCommand : ActivateTypeCommandBase { }
    public class ActivateDocumentTypeCommandHandler : ICommandHandler<ActivateDocumentTypeCommand>
    {
        private readonly IDocumentTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public ActivateDocumentTypeCommandHandler(IDocumentTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(ActivateDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"DocumentType with ID {request.Id} not found");
            entity.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

namespace Dualcomp.Auth.Application.DocumentTypes.DeactivateDocumentType
{
    public class DeactivateDocumentTypeCommand : DeactivateTypeCommandBase { }
    public class DeactivateDocumentTypeCommandHandler : ICommandHandler<DeactivateDocumentTypeCommand>
    {
        private readonly IDocumentTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public DeactivateDocumentTypeCommandHandler(IDocumentTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(DeactivateDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"DocumentType with ID {request.Id} not found");
            entity.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
