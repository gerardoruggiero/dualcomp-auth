using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.AddressTypes.UpdateAddressType
{
    public record UpdateAddressTypeCommand(Guid Id, string Name, string? Description = null, bool IsActive = true) 
        : ICommand<UpdateAddressTypeResult>, IUpdateTypeCommand;

    public record UpdateAddressTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<AddressTypeEntity>(Id, Name, Description, IsActive);
}
