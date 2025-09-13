using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Users.GetUsers
{
    public record GetUsersQuery(
        Guid? CompanyId = null,
        int Page = 1,
        int PageSize = 10,
        string? SearchTerm = null
    ) : IQuery<GetUsersResult>;

    public record GetUsersResult(
        List<UserDto> Users,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );

    public record UserDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        Guid CompanyId,
        bool IsActive,
        bool IsEmailValidated,
        bool MustChangePassword,
        bool IsCompanyAdmin,
        DateTime CreatedAt,
        DateTime? EmailValidatedAt
    );
}
