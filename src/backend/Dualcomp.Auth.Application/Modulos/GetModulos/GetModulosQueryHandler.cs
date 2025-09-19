using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.Application.Modulos.GetModulos;

public class GetModulosQueryHandler : IQueryHandler<GetModulosQuery, GetModulosResult>
{
	private readonly IModuloRepository _moduloRepository;

	public GetModulosQueryHandler(IModuloRepository moduloRepository)
	{
		_moduloRepository = moduloRepository ?? throw new ArgumentNullException(nameof(moduloRepository));
	}

	public async Task<GetModulosResult> Handle(GetModulosQuery query, CancellationToken cancellationToken)
	{
		var modulos = await _moduloRepository.GetAllAsync(cancellationToken);
		
		var moduloItems = modulos.Select(modulo => new ModuloItem(
			modulo.Id.ToString(),
			modulo.Name,
			modulo.Description,
			modulo.IsActive
		));

		return new GetModulosResult(moduloItems);
	}
}
