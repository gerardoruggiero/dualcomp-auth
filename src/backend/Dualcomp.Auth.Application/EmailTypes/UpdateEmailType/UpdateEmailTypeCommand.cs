using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.EmailTypes.UpdateEmailType
{
    public record UpdateEmailTypeCommand(Guid Id, string Name, string? Description = null, bool IsActive = true) 
        : ICommand<UpdateEmailTypeResult>, IUpdateTypeCommand;

    public record UpdateEmailTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<EmailTypeEntity>(Id, Name, Description, IsActive);
}
