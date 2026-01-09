using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.UpdateUser
{
    public record UpdateUserCommand(
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        Guid CompanyId,
        bool IsCompanyAdmin,
        Guid UpdatedBy
    ) : ICommand<UpdateUserResult>;

    public record UpdateUserResult(
        Guid UserId,
        string Email,
        string FullName,
        bool IsCompanyAdmin
    );
}
