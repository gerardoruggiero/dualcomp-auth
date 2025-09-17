using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.Application.Users.GetUsers
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, GetUsersResult>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<GetUsersResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            // Validar parámetros
            if (request.Page < 1) throw new ArgumentException("Page must be greater than 0", nameof(request.Page));
            if (request.PageSize < 1 || request.PageSize > 100) throw new ArgumentException("PageSize must be between 1 and 100", nameof(request.PageSize));

            // Obtener usuarios con paginación
            var users = await _userRepository.GetUsersAsync(
                request.CompanyId,
                request.Page,
                request.PageSize,
                request.SearchTerm,
                cancellationToken);

            var totalCount = await _userRepository.GetUsersCountAsync(
                request.CompanyId,
                request.SearchTerm,
                cancellationToken);

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var userDtos = users.Select(user => new UserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email.Value,
                user.CompanyId ?? Guid.Empty, // Handle nullable CompanyId
                user.IsActive,
                user.IsEmailValidated,
                user.MustChangePassword,
                user.IsCompanyAdmin,
                user.CreatedAt,
                user.EmailValidatedAt
            )).ToList();

            return new GetUsersResult(
                userDtos,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages
            );
        }
    }
}
