using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.DocumentTypes.UpdateDocumentType
{
    public record UpdateDocumentTypeCommand(Guid Id, string Name, string? Description = null, bool IsActive = true) 
        : ICommand<UpdateDocumentTypeResult>, IUpdateTypeCommand;

    public record UpdateDocumentTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<DocumentTypeEntity>(Id, Name, Description, IsActive);
}

