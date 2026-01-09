using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.PhoneTypes.ActivatePhoneType
{
    public class ActivatePhoneTypeCommand : ActivateTypeCommandBase { }
    public class ActivatePhoneTypeCommandHandler : ICommandHandler<ActivatePhoneTypeCommand>
    {
        private readonly IPhoneTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public ActivatePhoneTypeCommandHandler(IPhoneTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(ActivatePhoneTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"PhoneType with ID {request.Id} not found");
            entity.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

namespace Dualcomp.Auth.Application.PhoneTypes.DeactivatePhoneType
{
    public class DeactivatePhoneTypeCommand : DeactivateTypeCommandBase { }
    public class DeactivatePhoneTypeCommandHandler : ICommandHandler<DeactivatePhoneTypeCommand>
    {
        private readonly IPhoneTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public DeactivatePhoneTypeCommandHandler(IPhoneTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(DeactivatePhoneTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"PhoneType with ID {request.Id} not found");
            entity.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
