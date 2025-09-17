using Dualcomp.Auth.Application.AddressTypes.GetAddressTypes;
using Dualcomp.Auth.Application.AddressTypes.CreateAddressType;
using Dualcomp.Auth.Application.AddressTypes.UpdateAddressType;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AddressTypesController : BaseTypesController<
		GetAddressTypesQuery, 
		GetAddressTypesResult,
		CreateAddressTypeCommand,
		CreateAddressTypeResult,
		UpdateAddressTypeCommand,
		UpdateAddressTypeResult>
	{
        public AddressTypesController(
			IQueryHandler<GetAddressTypesQuery, GetAddressTypesResult> getAddressTypesHandler,
			ICommandHandler<CreateAddressTypeCommand, CreateAddressTypeResult> createAddressTypeHandler,
			ICommandHandler<UpdateAddressTypeCommand, UpdateAddressTypeResult> updateAddressTypeHandler) 
            : base(getAddressTypesHandler, createAddressTypeHandler, updateAddressTypeHandler)
        {
        }
	}
}
