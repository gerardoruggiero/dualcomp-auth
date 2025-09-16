using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Application.Companies.UpdateCompany;
using Dualcomp.Auth.Application.Companies.GetCompany;
using Dualcomp.Auth.Application.Companies.GetCompanies;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dualcomp.Auth.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CompaniesController : ControllerBase
	{
		private readonly ICommandHandler<RegisterCompanyCommand, RegisterCompanyResult> _registerCompanyHandler;
		private readonly ICommandHandler<UpdateCompanyCommand, UpdateCompanyResult> _updateCompanyHandler;
		private readonly IQueryHandler<GetCompanyQuery, GetCompanyResult> _getCompanyHandler;
		private readonly IQueryHandler<GetCompaniesQuery, GetCompaniesResult> _getCompaniesHandler;

		public CompaniesController(
			ICommandHandler<RegisterCompanyCommand, RegisterCompanyResult> registerCompanyHandler,
			ICommandHandler<UpdateCompanyCommand, UpdateCompanyResult> updateCompanyHandler,
			IQueryHandler<GetCompanyQuery, GetCompanyResult> getCompanyHandler,
			IQueryHandler<GetCompaniesQuery, GetCompaniesResult> getCompaniesHandler)
		{
			_registerCompanyHandler = registerCompanyHandler ?? throw new ArgumentNullException(nameof(registerCompanyHandler));
			_updateCompanyHandler = updateCompanyHandler ?? throw new ArgumentNullException(nameof(updateCompanyHandler));
			_getCompanyHandler = getCompanyHandler ?? throw new ArgumentNullException(nameof(getCompanyHandler));
			_getCompaniesHandler = getCompaniesHandler ?? throw new ArgumentNullException(nameof(getCompaniesHandler));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] RegisterCompanyCommand request, CancellationToken cancellationToken)
		{
			if (request == null) return BadRequest();
			
			try
			{
				var result = await _registerCompanyHandler.Handle(request, cancellationToken);
				return CreatedAtAction(nameof(GetById), new { id = result.CompanyId }, result);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var command = new UpdateCompanyCommand(
					id,
					request.Name,
					TaxId.Create(request.TaxId),
					request.Addresses.Select(a => new UpdateCompanyAddressDto(a.Id, Guid.Parse(a.AddressTypeId), a.Address, a.IsPrimary)).ToList(),
					request.Emails.Select(e => new UpdateCompanyEmailDto(e.Id, Guid.Parse(e.EmailTypeId), e.Email, e.IsPrimary)).ToList(),
					request.Phones.Select(p => new UpdateCompanyPhoneDto(p.Id, Guid.Parse(p.PhoneTypeId), p.Phone, p.IsPrimary)).ToList(),
					request.SocialMedias.Select(sm => new UpdateCompanySocialMediaDto(sm.Id, Guid.Parse(sm.SocialMediaTypeId), sm.Url, sm.IsPrimary)).ToList(),
					request.Employees.Select(e => new UpdateCompanyEmployeeDto(e.Id, e.FullName, e.Email, e.Phone, e.Position, e.HireDate)).ToList());

				var result = await _updateCompanyHandler.Handle(command, cancellationToken);

				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				var query = new GetCompanyQuery(id);
				var result = await _getCompanyHandler.Handle(query, cancellationToken);
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetCompanies(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] string? searchTerm = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				var query = new GetCompaniesQuery(page, pageSize, searchTerm);
				var result = await _getCompaniesHandler.Handle(query, cancellationToken);

				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Error interno del servidor" });
			}
		}
	}

	public record UpdateCompanyRequest(
		string Name, 
		string TaxId, 
		List<UpdateCompanyAddressRequest> Addresses,
		List<UpdateCompanyEmailRequest> Emails,
		List<UpdateCompanyPhoneRequest> Phones,
		List<UpdateCompanySocialMediaRequest> SocialMedias,
		List<UpdateCompanyEmployeeRequest> Employees
	);

	public record UpdateCompanyAddressRequest(
		Guid? Id,
		string AddressTypeId,
		string Address,
		bool IsPrimary
	);

	public record UpdateCompanyEmailRequest(
		Guid? Id,
		string EmailTypeId,
		string Email,
		bool IsPrimary
	);

	public record UpdateCompanyPhoneRequest(
		Guid? Id,
		string PhoneTypeId,
		string Phone,
		bool IsPrimary
	);

	public record UpdateCompanySocialMediaRequest(
		Guid? Id,
		string SocialMediaTypeId,
		string Url,
		bool IsPrimary
	);

	public record UpdateCompanyEmployeeRequest(
		Guid? Id,
		string FullName,
		string Email,
		string? Phone,
		string? Position,
		DateTime? HireDate
	);
}
