using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.AddressTypes.GetAddressTypes
{
	public class GetAddressTypesQueryHandler : GetTypesQueryHandler<AddressTypeEntity, IAddressTypeRepository, GetAddressTypesQuery, GetAddressTypesResult>
	{
        public GetAddressTypesQueryHandler(IAddressTypeRepository addressTypeRepository) 
            : base(
                addressTypeRepository,
                (repo, ct) => repo.GetAllAsync(ct),
                entities => new GetAddressTypesResult(entities.Select(e => new AddressTypeItem(e.Id.ToString(), e.Name))))
        {
        }
	}
}
