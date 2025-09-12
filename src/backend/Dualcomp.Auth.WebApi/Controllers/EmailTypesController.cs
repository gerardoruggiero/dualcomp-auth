using Dualcomp.Auth.Application.EmailTypes.GetEmailTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EmailTypesController : BaseTypesController<GetEmailTypesQuery, GetEmailTypesResult>
	{
        public EmailTypesController(IQueryHandler<GetEmailTypesQuery, GetEmailTypesResult> getEmailTypesHandler) 
            : base(getEmailTypesHandler)
        {
        }
	}
}
