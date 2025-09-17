using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;

namespace Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes
{
	public class GetSocialMediaTypesQueryHandler : GetTypesQueryHandler<SocialMediaTypeEntity, ISocialMediaTypeRepository, GetSocialMediaTypesQuery, GetSocialMediaTypesResult>
	{
        public GetSocialMediaTypesQueryHandler(ISocialMediaTypeRepository socialMediaTypeRepository) 
            : base(
                socialMediaTypeRepository,
                (repo, ct) => repo.GetAllAsync(ct),
                entities => new GetSocialMediaTypesResult(entities.Select(e => new SocialMediaTypeItem(e.Id.ToString(), e.Name, e.Description, e.IsActive))))
        {
        }
	}
}
