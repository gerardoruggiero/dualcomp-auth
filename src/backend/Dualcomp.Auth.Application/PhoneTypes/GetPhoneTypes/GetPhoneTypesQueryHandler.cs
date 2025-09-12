using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes
{
	public class GetPhoneTypesQueryHandler : IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult>
	{
		private readonly IPhoneTypeRepository _phoneTypeRepository;

        public GetPhoneTypesQueryHandler(IPhoneTypeRepository phoneTypeRepository) => _phoneTypeRepository = phoneTypeRepository ?? throw new ArgumentNullException(nameof(phoneTypeRepository));

        public async Task<GetPhoneTypesResult> Handle(GetPhoneTypesQuery request, CancellationToken cancellationToken)
		{
			var phoneTypes = await _phoneTypeRepository.GetAllAsync(cancellationToken);

			var phoneTypeItems = phoneTypes.Select(pt => new PhoneTypeItem(pt.Name)).ToList();

			return new GetPhoneTypesResult(phoneTypeItems);
		}
	}
}
