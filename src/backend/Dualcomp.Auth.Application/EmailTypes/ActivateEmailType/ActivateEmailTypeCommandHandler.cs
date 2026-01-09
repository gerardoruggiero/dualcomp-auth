using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.EmailTypes.ActivateEmailType
{
    public class ActivateEmailTypeCommandHandler : ICommandHandler<ActivateEmailTypeCommand>
    {
        private readonly IEmailTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateEmailTypeCommandHandler(IEmailTypeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(ActivateEmailTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                throw new KeyNotFoundException($"EmailType with ID {request.Id} not found");
            }

            entity.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
