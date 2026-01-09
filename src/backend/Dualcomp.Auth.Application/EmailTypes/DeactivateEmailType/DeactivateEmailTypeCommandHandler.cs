using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.EmailTypes.DeactivateEmailType
{
    public class DeactivateEmailTypeCommandHandler : ICommandHandler<DeactivateEmailTypeCommand>
    {
        private readonly IEmailTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateEmailTypeCommandHandler(IEmailTypeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(DeactivateEmailTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                throw new KeyNotFoundException($"EmailType with ID {request.Id} not found");
            }

            entity.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
