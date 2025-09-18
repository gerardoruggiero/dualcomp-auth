using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.DocumentTypes.UpdateDocumentType
{
    public class UpdateDocumentTypeCommandHandler : ICommandHandler<UpdateDocumentTypeCommand, UpdateDocumentTypeResult>
    {
        private readonly IDocumentTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDocumentTypeCommandHandler(IDocumentTypeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UpdateDocumentTypeResult> Handle(UpdateDocumentTypeCommand command, CancellationToken cancellationToken)
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
                throw new InvalidOperationException($"DocumentType with ID {command.Id} not found");

            // Validar unicidad del nombre (excluyendo la entidad actual)
            await ValidateNameUniqueness(command.Name, command.Id, cancellationToken);

            // Actualizar entidad
            entity.UpdateInfo(command.Name, command.Description, command.IsActive);

            // Guardar cambios
            await _repository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateDocumentTypeResult(entity.Id, entity.Name, entity.Description, entity.IsActive);
        }

        private async Task ValidateNameUniqueness(string name, Guid excludeId, CancellationToken cancellationToken)
        {
            var existing = await _repository.ListAsync(x => x.Name == name && x.Id != excludeId, cancellationToken);
            if (existing.Any())
            {
                throw new InvalidOperationException($"A DocumentType with name '{name}' already exists");
            }
        }
    }
}

