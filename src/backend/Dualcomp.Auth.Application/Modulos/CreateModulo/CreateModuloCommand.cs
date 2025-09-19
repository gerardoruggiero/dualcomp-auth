using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Modulos.CreateModulo;

public record CreateModuloCommand(string Name, string? Description = null) : ICommand<CreateModuloResult>, ICreateTypeCommand;

public record CreateModuloResult(Guid Id, string Name, string? Description, bool IsActive) : BaseTypeResult<ModuloEntity>(Id, Name, Description, IsActive);
