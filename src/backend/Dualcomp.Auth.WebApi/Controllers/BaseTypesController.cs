using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Abstractions.Commands;
using Dualcomp.Auth.Domain.Companies;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
    /// <summary>
    /// Controller base genérico para operaciones CRUD de tipos de entidades
    /// </summary>
    /// <typeparam name="TQuery">Tipo de query</typeparam>
    /// <typeparam name="TResult">Tipo de resultado</typeparam>
    /// <typeparam name="TCreateCommand">Tipo de comando de creación</typeparam>
    /// <typeparam name="TCreateResult">Tipo de resultado de creación</typeparam>
    /// <typeparam name="TUpdateCommand">Tipo de comando de actualización</typeparam>
    /// <typeparam name="TUpdateResult">Tipo de resultado de actualización</typeparam>
    public abstract class BaseTypesController<TQuery, TResult, TCreateCommand, TCreateResult, TUpdateCommand, TUpdateResult> : ControllerBase
        where TQuery : IQuery<TResult>, new()
        where TResult : class
        where TCreateCommand : ICommand<TCreateResult>
        where TCreateResult : class
        where TUpdateCommand : ICommand<TUpdateResult>
        where TUpdateResult : class
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;
        private readonly ICommandHandler<TCreateCommand, TCreateResult> _createCommandHandler;
        private readonly ICommandHandler<TUpdateCommand, TUpdateResult> _updateCommandHandler;

        protected BaseTypesController(
            IQueryHandler<TQuery, TResult> queryHandler,
            ICommandHandler<TCreateCommand, TCreateResult> createCommandHandler,
            ICommandHandler<TUpdateCommand, TUpdateResult> updateCommandHandler)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _createCommandHandler = createCommandHandler ?? throw new ArgumentNullException(nameof(createCommandHandler));
            _updateCommandHandler = updateCommandHandler ?? throw new ArgumentNullException(nameof(updateCommandHandler));
        }

        /// <summary>
        /// Obtiene todos los tipos de la entidad
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de tipos</returns>
        [HttpGet]
        public async Task<IActionResult> GetTypes(CancellationToken cancellationToken)
        {
            try
            {
                var query = new TQuery();
                var result = await _queryHandler.Handle(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de entidad
        /// </summary>
        /// <param name="command">Comando de creación</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la creación</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TCreateCommand command, CancellationToken cancellationToken)
        {
            if (command == null) return BadRequest();

            try
            {
                var result = await _createCommandHandler.Handle(command, cancellationToken);
                return CreatedAtAction(nameof(GetTypes), result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        /// <summary>
        /// Actualiza un tipo de entidad existente
        /// </summary>
        /// <param name="id">ID del tipo a actualizar</param>
        /// <param name="command">Comando de actualización</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la actualización</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TUpdateCommand command, CancellationToken cancellationToken)
        {
            if (command == null) return BadRequest();

            // Validar que el ID del comando coincida con el ID de la URL
            var commandId = GetCommandId(command);
            if (commandId != id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                var result = await _updateCommandHandler.Handle(command, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        /// <summary>
        /// Obtiene el ID del comando usando reflexión
        /// </summary>
        private static Guid GetCommandId(TUpdateCommand command)
        {
            var idProperty = typeof(TUpdateCommand).GetProperty("Id");
            if (idProperty != null && idProperty.GetValue(command) is Guid id)
            {
                return id;
            }
            throw new InvalidOperationException("Command does not have an Id property");
        }
    }
}
