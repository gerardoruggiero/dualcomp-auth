using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.PhoneTypes.UpdatePhoneType
{
    public record UpdatePhoneTypeCommand(Guid Id, string Name, string? Description = null, bool IsActive = true) 
        : ICommand<UpdatePhoneTypeResult>, IUpdateTypeCommand;

    public record UpdatePhoneTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<PhoneTypeEntity>(Id, Name, Description, IsActive);
}
