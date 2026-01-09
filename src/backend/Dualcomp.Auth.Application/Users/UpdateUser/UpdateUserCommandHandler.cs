using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Users.UpdateUser
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            // Validar que el email no esté en uso por otro usuario
            var email = Email.Create(request.Email);
            var existingUserWithEmail = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            user.UpdateProfile(request.FirstName, request.LastName, email);
            user.SetCompanyAdmin(request.IsCompanyAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateUserResult(
                user.Id,
                user.Email.Value,
                user.GetFullName(),
                user.IsCompanyAdmin
            );
        }
    }
}
