using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.DocumentTypes.CreateDocumentType
{
    public record CreateDocumentTypeCommand(string Name, string? Description = null) 
        : ICommand<CreateDocumentTypeResult>, ICreateTypeCommand;

    public record CreateDocumentTypeResult(Guid Id, string Name, string? Description, bool IsActive) 
        : BaseTypeResult<DocumentTypeEntity>(Id, Name, Description, IsActive);
}

