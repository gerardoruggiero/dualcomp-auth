using Dualcomp.Auth.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Dualcomp.Auth.WebApi.Controllers
{
    /// <summary>
    /// Controller base genérico para obtener tipos de entidades
    /// </summary>
    /// <typeparam name="TQuery">Tipo de query</typeparam>
    /// <typeparam name="TResult">Tipo de resultado</typeparam>
    public abstract class BaseTypesController<TQuery, TResult> : ControllerBase
        where TQuery : IQuery<TResult>, new()
        where TResult : class
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;

        protected BaseTypesController(IQueryHandler<TQuery, TResult> queryHandler)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
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
    }
}
