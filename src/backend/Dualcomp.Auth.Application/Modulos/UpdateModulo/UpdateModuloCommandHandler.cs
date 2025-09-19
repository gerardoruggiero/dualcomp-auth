using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Modulos.UpdateModulo;

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
		var modulo = await _moduloRepository.GetByIdAsync(command.Id, cancellationToken);
		
		if (modulo == null)
		{
			throw new InvalidOperationException($"Modulo with ID {command.Id} not found.");
		}

		modulo.UpdateInfo(command.Name, command.Description, command.IsActive);

		await _moduloRepository.UpdateAsync(modulo, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return new UpdateModuloResult(modulo.Id, modulo.Name, modulo.Description, modulo.IsActive);
	}
}
