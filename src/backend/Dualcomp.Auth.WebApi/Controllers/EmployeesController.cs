using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dualcomp.Auth.Application.Employees.CreateEmployee;
using Dualcomp.Auth.Application.Employees.UpdateEmployee;
using Dualcomp.Auth.Application.Employees.GetEmployee;
using Dualcomp.Auth.Application.Employees.GetEmployees;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;

namespace Dualcomp.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly ICommandHandler<CreateEmployeeCommand, CreateEmployeeResult> _createEmployeeHandler;
        private readonly ICommandHandler<UpdateEmployeeCommand, UpdateEmployeeResult> _updateEmployeeHandler;
        private readonly IQueryHandler<GetEmployeeQuery, GetEmployeeResult> _getEmployeeHandler;
        private readonly IQueryHandler<GetEmployeesQuery, GetEmployeesResult> _getEmployeesHandler;

        public EmployeesController(
            ICommandHandler<CreateEmployeeCommand, CreateEmployeeResult> createEmployeeHandler,
            ICommandHandler<UpdateEmployeeCommand, UpdateEmployeeResult> updateEmployeeHandler,
            IQueryHandler<GetEmployeeQuery, GetEmployeeResult> getEmployeeHandler,
            IQueryHandler<GetEmployeesQuery, GetEmployeesResult> getEmployeesHandler)
        {
            _createEmployeeHandler = createEmployeeHandler ?? throw new ArgumentNullException(nameof(createEmployeeHandler));
            _updateEmployeeHandler = updateEmployeeHandler ?? throw new ArgumentNullException(nameof(updateEmployeeHandler));
            _getEmployeeHandler = getEmployeeHandler ?? throw new ArgumentNullException(nameof(getEmployeeHandler));
            _getEmployeesHandler = getEmployeesHandler ?? throw new ArgumentNullException(nameof(getEmployeesHandler));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new CreateEmployeeCommand(
                    request.FullName,
                    Email.Create(request.Email),
                    request.Phone,
                    request.CompanyId,
                    request.Position,
                    request.HireDate,
                    request.UserId);

                var result = await _createEmployeeHandler.Handle(command, HttpContext.RequestAborted);

                return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new UpdateEmployeeCommand(
                    id,
                    request.FullName,
                    Email.Create(request.Email),
                    request.Phone,
                    request.Position);

                var result = await _updateEmployeeHandler.Handle(command, HttpContext.RequestAborted);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            try
            {
                var query = new GetEmployeeQuery(id);
                var result = await _getEmployeeHandler.Handle(query, HttpContext.RequestAborted);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] Guid? companyId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var query = new GetEmployeesQuery(companyId, page, pageSize, searchTerm);
                var result = await _getEmployeesHandler.Handle(query, HttpContext.RequestAborted);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }
    }

    public record CreateEmployeeRequest(
        string FullName,
        string Email,
        string? Phone,
        Guid CompanyId,
        string? Position = null,
        DateTime? HireDate = null,
        Guid? UserId = null);

    public record UpdateEmployeeRequest(
        string FullName,
        string Email,
        string? Phone,
        string? Position);
}
