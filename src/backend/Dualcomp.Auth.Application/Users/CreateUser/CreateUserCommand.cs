using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.CreateUser
{
    public record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        Guid CompanyId,
        bool IsCompanyAdmin = false,
        Guid? CreatedBy = null
    ) : ICommand<CreateUserResult>;

    public record CreateUserResult(
        Guid UserId,
        string Email,
        string FullName,
        string TemporaryPassword,
        bool IsCompanyAdmin
    );
}
