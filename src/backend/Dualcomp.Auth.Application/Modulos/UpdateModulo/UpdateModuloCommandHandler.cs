using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Modulos.UpdateModulo
{
    public class UpdateModuloCommandHandler : ICommandHandler<UpdateModuloCommand, UpdateModuloResult>
    {
    	private readonly IModuloRepository _moduloRepository;
    	private readonly IUnitOfWork _unitOfWork;

    	public UpdateModuloCommandHandler(IModuloRepository moduloRepository, IUnitOfWork unitOfWork)
    	{
    		_moduloRepository = moduloRepository ?? throw new ArgumentNullException(nameof(moduloRepository));
    		_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    	}

    	public async Task<UpdateModuloResult> Handle(UpdateModuloCommand command, CancellationToken cancellationToken)
    	{
    		// Validaciones comunes
    		if (string.IsNullOrWhiteSpace(command.Name))
    			throw new ArgumentException("Name is required", nameof(command.Name));

    		if (command.Name.Length > 50)
    			throw new ArgumentException("Name cannot exceed 50 characters", nameof(command.Name));

    		if (!string.IsNullOrWhiteSpace(command.Description) && command.Description.Length > 200)
    			throw new ArgumentException("Description cannot exceed 200 characters", nameof(command.Description));

            // Buscar entidad existente
            var modulo = await _moduloRepository.GetByIdAsync(command.Id, cancellationToken);
    		if (modulo == null)
    			throw new InvalidOperationException($"Modulo with ID {command.Id} not found.");

            // Validar unicidad del nombre (excluyendo la entidad actual)
            await ValidateNameUniqueness(command.Name, command.Id, cancellationToken);

            // Actualizar entidad
            modulo.UpdateInfo(command.Name, command.Description, command.IsActive);

            // Guardar cambios
            await _moduloRepository.UpdateAsync(modulo, cancellationToken);
    		await _unitOfWork.SaveChangesAsync(cancellationToken);

    		return new UpdateModuloResult(modulo.Id, modulo.Name, modulo.Description, modulo.IsActive);
    	}

        private async Task ValidateNameUniqueness(string name, Guid excludeId, CancellationToken cancellationToken)
        {
            var existing = await _moduloRepository.ListAsync(x => x.Name == name && x.Id != excludeId, cancellationToken);
            if (existing.Any())
            {
                throw new InvalidOperationException($"A PhoneType with name '{name}' already exists");
            }
        }
    }
}
