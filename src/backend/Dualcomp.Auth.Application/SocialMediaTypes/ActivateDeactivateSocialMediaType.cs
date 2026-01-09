using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.SocialMediaTypes.ActivateSocialMediaType
{
    public class ActivateSocialMediaTypeCommand : ActivateTypeCommandBase { }
    public class ActivateSocialMediaTypeCommandHandler : ICommandHandler<ActivateSocialMediaTypeCommand>
    {
        private readonly ISocialMediaTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public ActivateSocialMediaTypeCommandHandler(ISocialMediaTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(ActivateSocialMediaTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"SocialMediaType with ID {request.Id} not found");
            entity.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

namespace Dualcomp.Auth.Application.SocialMediaTypes.DeactivateSocialMediaType
{
    public class DeactivateSocialMediaTypeCommand : DeactivateTypeCommandBase { }
    public class DeactivateSocialMediaTypeCommandHandler : ICommandHandler<DeactivateSocialMediaTypeCommand>
    {
        private readonly ISocialMediaTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public DeactivateSocialMediaTypeCommandHandler(ISocialMediaTypeRepository repository, IUnitOfWork unitOfWork) { _repository = repository; _unitOfWork = unitOfWork; }
        public async Task Handle(DeactivateSocialMediaTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException($"SocialMediaType with ID {request.Id} not found");
            entity.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
