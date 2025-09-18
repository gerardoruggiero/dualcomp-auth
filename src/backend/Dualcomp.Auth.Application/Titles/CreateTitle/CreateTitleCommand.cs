using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Titles.CreateTitle;

public record CreateTitleCommand(string Name, string? Description = null) : ICommand<CreateTitleResult>, ICreateTypeCommand;

public record CreateTitleResult(Guid Id, string Name, string? Description, bool IsActive) : BaseTypeResult<TitleEntity>(Id, Name, Description, IsActive);
