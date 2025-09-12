using Dualcomp.Auth.Application.PhoneTypes.GetPhoneTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PhoneTypesController : BaseTypesController<GetPhoneTypesQuery, GetPhoneTypesResult>
	{
        public PhoneTypesController(IQueryHandler<GetPhoneTypesQuery, GetPhoneTypesResult> getPhoneTypesHandler) 
            : base(getPhoneTypesHandler)
        {
        }
	}
}
