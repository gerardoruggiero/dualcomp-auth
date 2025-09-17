using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.PhoneTypes.CreatePhoneType
{
    public record CreatePhoneTypeCommand(string Name, string? Description = null) 
        : ICommand<CreatePhoneTypeResult>, ICreateTypeCommand;

    public record CreatePhoneTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<PhoneTypeEntity>(Id, Name, Description, IsActive);
}
