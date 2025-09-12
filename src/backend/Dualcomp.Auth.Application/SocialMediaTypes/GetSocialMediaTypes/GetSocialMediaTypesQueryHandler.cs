using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.SocialMediaTypes.GetSocialMediaTypes
{
	public class GetSocialMediaTypesQueryHandler : IQueryHandler<GetSocialMediaTypesQuery, GetSocialMediaTypesResult>
	{
		private readonly ISocialMediaTypeRepository _socialMediaTypeRepository;

        public GetSocialMediaTypesQueryHandler(ISocialMediaTypeRepository socialMediaTypeRepository) => _socialMediaTypeRepository = socialMediaTypeRepository ?? throw new ArgumentNullException(nameof(socialMediaTypeRepository));

        public async Task<GetSocialMediaTypesResult> Handle(GetSocialMediaTypesQuery request, CancellationToken cancellationToken)
		{
			var socialMediaTypes = await _socialMediaTypeRepository.GetAllAsync(cancellationToken);

			var socialMediaTypeItems = socialMediaTypes.Select(smt => new SocialMediaTypeItem(smt.Name)).ToList();

			return new GetSocialMediaTypesResult(socialMediaTypeItems);
		}
	}
}
