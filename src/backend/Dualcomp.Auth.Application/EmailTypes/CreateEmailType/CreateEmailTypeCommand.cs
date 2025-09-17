using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.EmailTypes.CreateEmailType
{
    public record CreateEmailTypeCommand(string Name, string? Description = null) 
        : ICommand<CreateEmailTypeResult>, ICreateTypeCommand;

    public record CreateEmailTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<EmailTypeEntity>(Id, Name, Description, IsActive);
}
