using Dualcomp.Auth.Application.AddressTypes.GetAddressTypes;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AddressTypesController : BaseTypesController<GetAddressTypesQuery, GetAddressTypesResult>
	{
        public AddressTypesController(IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult> getAddressTypesHandler) 
            : base(getAddressTypesHandler)
        {
        }
	}
}
