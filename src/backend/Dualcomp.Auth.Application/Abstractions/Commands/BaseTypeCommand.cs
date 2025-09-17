using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Abstractions.Commands
{
    /// <summary>
    /// Comando base genérico para crear tipos de entidades
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    public abstract record BaseCreateTypeCommand<TEntity>(string Name, string? Description = null) 
        : ICommand<BaseTypeResult<TEntity>>, ICreateTypeCommand
        where TEntity : BaseTypeEntity;

    /// <summary>
    /// Comando base genérico para actualizar tipos de entidades
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    public abstract record BaseUpdateTypeCommand<TEntity>(Guid Id, string Name, string? Description = null) 
        : ICommand<BaseTypeResult<TEntity>>, IUpdateTypeCommand
        where TEntity : BaseTypeEntity;
}
