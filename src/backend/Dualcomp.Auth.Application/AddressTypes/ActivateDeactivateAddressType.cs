using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

// AddressType
namespace Dualcomp.Auth.Application.AddressTypes.ActivateAddressType
{
    public class ActivateAddressTypeCommand : ActivateTypeCommandBase { }
    public class ActivateAddressTypeCommandHandler : ICommandHandler<ActivateAddressTypeCommand>
    {
        private readonly IAddressTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public ActivateAddressTypeCommandHandler(IAddressTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(ActivateAddressTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"AddressType with ID {request.Id} not found");
            entity.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

namespace Dualcomp.Auth.Application.AddressTypes.DeactivateAddressType
{
    public class DeactivateAddressTypeCommand : DeactivateTypeCommandBase { }
    public class DeactivateAddressTypeCommandHandler : ICommandHandler<DeactivateAddressTypeCommand>
    {
        private readonly IAddressTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public DeactivateAddressTypeCommandHandler(IAddressTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(DeactivateAddressTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"AddressType with ID {request.Id} not found");
            entity.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
