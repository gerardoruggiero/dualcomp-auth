using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.AddressTypes.GetAddressTypes
{
	public class GetAddressTypesQueryHandler : IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult>
	{
		private readonly IAddressTypeRepository _addressTypeRepository;

        public GetAddressTypesQueryHandler(IAddressTypeRepository addressTypeRepository) => _addressTypeRepository = addressTypeRepository ?? throw new ArgumentNullException(nameof(addressTypeRepository));

        public async Task<GetAddressTypesResult> Handle(GetAddressTypesQuery request, CancellationToken cancellationToken)
		{
			var addressTypes = await _addressTypeRepository.GetAllAsync(cancellationToken);

			var addressTypeItems = addressTypes.Select(at => new AddressTypeItem(at.Name)).ToList();

			return new GetAddressTypesResult(addressTypeItems);
		}
	}
}
