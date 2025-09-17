namespace Dualcomp.Auth.Application.Abstractions.Commands
{
    /// <summary>
    /// Resultado base genérico para operaciones de tipos de entidades
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    public record BaseTypeResult<TEntity>(Guid Id, string Name, string? Description, bool IsActive);
}
