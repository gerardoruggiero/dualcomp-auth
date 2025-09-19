using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Modulos.UpdateModulo;

public record UpdateModuloCommand(Guid Id, string Name, string? Description = null, bool IsActive = true) 
    : ICommand<UpdateModuloResult>, IUpdateTypeCommand;

public record UpdateModuloResult(Guid Id, string Name, string? Description, bool IsActive) 
    : BaseTypeResult<ModuloEntity>(Id, Name, Description, IsActive);
