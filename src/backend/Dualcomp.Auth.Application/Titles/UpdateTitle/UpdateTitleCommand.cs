using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Titles.UpdateTitle
{
    public record UpdateTitleCommand(Guid Id, string Name, string? Description = null, bool IsActive = true) 
        : ICommand<UpdateTitleResult>, IUpdateTypeCommand;

    public record UpdateTitleResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<TitleEntity>(Id, Name, Description, IsActive);
}

