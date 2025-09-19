using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Modulos.CreateModulo;

public class CreateModuloCommandHandler : ICommandHandler<CreateModuloCommand, CreateModuloResult>
{
	private readonly IModuloRepository _moduloRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateModuloCommandHandler(IModuloRepository moduloRepository, IUnitOfWork unitOfWork)
	{
		_moduloRepository = moduloRepository ?? throw new ArgumentNullException(nameof(moduloRepository));
		_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<CreateModuloResult> Handle(CreateModuloCommand command, CancellationToken cancellationToken)
	{
		var modulo = ModuloEntity.Create(command.Name, command.Description);

		await _moduloRepository.AddAsync(modulo, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return new CreateModuloResult(modulo.Id, modulo.Name, modulo.Description, modulo.IsActive);
	}
}
