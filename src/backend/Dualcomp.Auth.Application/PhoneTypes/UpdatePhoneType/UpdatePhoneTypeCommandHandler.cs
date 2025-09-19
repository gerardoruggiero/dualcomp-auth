using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.PhoneTypes.UpdatePhoneType
{
    public class UpdatePhoneTypeCommandHandler : ICommandHandler<UpdatePhoneTypeCommand, UpdatePhoneTypeResult>
    {
        private readonly IPhoneTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePhoneTypeCommandHandler(IPhoneTypeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UpdatePhoneTypeResult> Handle(UpdatePhoneTypeCommand command, CancellationToken cancellationToken)
        {
            // Validaciones comunes
            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Name is required", nameof(command.Name));

            if (command.Name.Length > 50)
                throw new ArgumentException("Name cannot exceed 50 characters", nameof(command.Name));

            if (!string.IsNullOrWhiteSpace(command.Description) && command.Description.Length > 200)
                throw new ArgumentException("Description cannot exceed 200 characters", nameof(command.Description));

            // Buscar entidad existente
            var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (entity == null)
                throw new InvalidOperationException($"PhoneType with ID {command.Id} not found");

            // Validar unicidad del nombre (excluyendo la entidad actual)
            await ValidateNameUniqueness(command.Name, command.Id, cancellationToken);

            // Actualizar entidad
            entity.UpdateInfo(command.Name, command.Description, command.IsActive);

            // Guardar cambios
            await _repository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdatePhoneTypeResult(entity.Id, entity.Name, entity.Description, entity.IsActive);
        }

        private async Task ValidateNameUniqueness(string name, Guid excludeId, CancellationToken cancellationToken)
        {
            var existing = await _repository.ListAsync(x => x.Name == name && x.Id != excludeId, cancellationToken);
            if (existing.Any())
            {
                throw new InvalidOperationException($"A PhoneType with name '{name}' already exists");
            }
        }
    }
}
