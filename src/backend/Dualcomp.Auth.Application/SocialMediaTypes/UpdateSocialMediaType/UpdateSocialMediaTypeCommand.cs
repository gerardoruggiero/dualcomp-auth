using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.SocialMediaTypes.UpdateSocialMediaType
{
    public record UpdateSocialMediaTypeCommand(Guid Id, string Name, string? Description = null, bool IsActive = true) 
        : ICommand<UpdateSocialMediaTypeResult>, IUpdateTypeCommand;

    public record UpdateSocialMediaTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<SocialMediaTypeEntity>(Id, Name, Description, IsActive);
}
