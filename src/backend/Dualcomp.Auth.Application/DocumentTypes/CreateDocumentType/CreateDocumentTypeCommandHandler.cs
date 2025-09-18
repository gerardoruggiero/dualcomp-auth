using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.DocumentTypes.CreateDocumentType
{
    public class CreateDocumentTypeCommandHandler : ICommandHandler<CreateDocumentTypeCommand, CreateDocumentTypeResult>
    {
        private readonly IDocumentTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDocumentTypeCommandHandler(IDocumentTypeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<CreateDocumentTypeResult> Handle(CreateDocumentTypeCommand command, CancellationToken cancellationToken)
        {
            // Validaciones comunes
            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Name is required", nameof(command.Name));

            if (command.Name.Length > 50)
                throw new ArgumentException("Name cannot exceed 50 characters", nameof(command.Name));

            if (!string.IsNullOrWhiteSpace(command.Description) && command.Description.Length > 200)
                throw new ArgumentException("Description cannot exceed 200 characters", nameof(command.Description));

            // Validar unicidad del nombre
            await ValidateNameUniqueness(command.Name, cancellationToken);

            // Crear entidad
            var entity = DocumentTypeEntity.Create(command.Name, command.Description);

            // Guardar entidad
            await _repository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateDocumentTypeResult(entity.Id, entity.Name, entity.Description, entity.IsActive);
        }

        private async Task ValidateNameUniqueness(string name, CancellationToken cancellationToken)
        {
            var existing = await _repository.ListAsync(x => x.Name == name, cancellationToken);
            if (existing.Any())
            {
                throw new InvalidOperationException($"A DocumentType with name '{name}' already exists");
            }
        }
    }
}

