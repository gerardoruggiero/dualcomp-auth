using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.EmailTypes.GetEmailTypes
{
	public class GetEmailTypesQueryHandler : IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult>
	{
		private readonly IEmailTypeRepository _emailTypeRepository;

        public GetEmailTypesQueryHandler(IEmailTypeRepository emailTypeRepository) => _emailTypeRepository = emailTypeRepository ?? throw new ArgumentNullException(nameof(emailTypeRepository));

        public async Task<GetEmailTypesResult> Handle(GetEmailTypesQuery request, CancellationToken cancellationToken)
		{
			var emailTypes = await _emailTypeRepository.GetAllAsync(cancellationToken);

			var emailTypeItems = emailTypes.Select(et => new EmailTypeItem(et.Name)).ToList();

			return new GetEmailTypesResult(emailTypeItems);
		}
	}
}
