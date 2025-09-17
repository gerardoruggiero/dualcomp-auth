using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.SocialMediaTypes.CreateSocialMediaType
{
    public record CreateSocialMediaTypeCommand(string Name, string? Description = null) 
        : ICommand<CreateSocialMediaTypeResult>, ICreateTypeCommand;

    public record CreateSocialMediaTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<SocialMediaTypeEntity>(Id, Name, Description, IsActive);
}
