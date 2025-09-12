using Dualcomp.Auth.Application.Abstractions.Messaging;
using DualComp.Infraestructure.Data.Persistence;

namespace Dualcomp.Auth.Application.Abstractions.Queries
{
    /// <summary>
    /// Handler genérico para obtener tipos de entidades
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    /// <typeparam name="TRepository">Tipo de repositorio</typeparam>
    /// <typeparam name="TQuery">Tipo de query</typeparam>
    /// <typeparam name="TResult">Tipo de resultado</typeparam>
    public class GetTypesQueryHandler<TEntity, TRepository, TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TEntity : class
        where TRepository : class
        where TQuery : IQuery<TResult>
        where TResult : class
    {
        private readonly TRepository _repository;
        private readonly Func<TRepository, CancellationToken, Task<IEnumerable<TEntity>>> _getAllAsync;
        private readonly Func<IEnumerable<TEntity>, TResult> _createResult;

        public GetTypesQueryHandler(
            TRepository repository,
            Func<TRepository, CancellationToken, Task<IEnumerable<TEntity>>> getAllAsync,
            Func<IEnumerable<TEntity>, TResult> createResult)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _getAllAsync = getAllAsync ?? throw new ArgumentNullException(nameof(getAllAsync));
            _createResult = createResult ?? throw new ArgumentNullException(nameof(createResult));
        }

        public async Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
        {
            var entities = await _getAllAsync(_repository, cancellationToken);
            return _createResult(entities);
        }
    }

    /// <summary>
    /// Resultado genérico para tipos
    /// </summary>
    /// <typeparam name="TItem">Tipo de item</typeparam>
    public record GetTypesResult<TItem>(IEnumerable<TItem> Items);

    /// <summary>
    /// Item genérico para tipos
    /// </summary>
    public record TypeItem(string Id, string Value);
}
