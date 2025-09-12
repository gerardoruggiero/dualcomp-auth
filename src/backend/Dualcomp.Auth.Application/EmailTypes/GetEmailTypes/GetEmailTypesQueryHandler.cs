using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Queries;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.EmailTypes.GetEmailTypes
{
	public class GetEmailTypesQueryHandler : GetTypesQueryHandler<EmailTypeEntity, IEmailTypeRepository, GetEmailTypesQuery, GetEmailTypesResult>
	{
        public GetEmailTypesQueryHandler(IEmailTypeRepository emailTypeRepository) 
            : base(
                emailTypeRepository,
                (repo, ct) => repo.GetAllAsync(ct),
                entities => new GetEmailTypesResult(entities.Select(e => new EmailTypeItem(e.Id.ToString(), e.Name))))
        {
        }
	}
}
