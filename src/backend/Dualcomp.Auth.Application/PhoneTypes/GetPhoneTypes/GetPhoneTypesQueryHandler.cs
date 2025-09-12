using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes
{
	public class GetPhoneTypesQueryHandler : GetTypesQueryHandler<PhoneTypeEntity, IPhoneTypeRepository, GetPhoneTypesQuery, GetPhoneTypesResult>
	{
        public GetPhoneTypesQueryHandler(IPhoneTypeRepository phoneTypeRepository) 
            : base(
                phoneTypeRepository,
                (repo, ct) => repo.GetAllAsync(ct),
                entities => new GetPhoneTypesResult(entities.Select(e => new PhoneTypeItem(e.Id.ToString(), e.Name))))
        {
        }
	}
}
