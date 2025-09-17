using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.AddressTypes.CreateAddressType
{
    public record CreateAddressTypeCommand(string Name, string? Description = null) 
        : ICommand<CreateAddressTypeResult>, ICreateTypeCommand;

    public record CreateAddressTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<AddressTypeEntity>(Id, Name, Description, IsActive);
}
