using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.Application.Abstractions.Commands
{
    /// <summary>
    /// Interfaz base para comandos de creación de tipos
    /// </summary>
    public interface ICreateTypeCommand : ICommand
    {
        string Name { get; }
        string? Description { get; }
    }

    /// <summary>
    /// Interfaz base para comandos de actualización de tipos
    /// </summary>
    public interface IUpdateTypeCommand : ICommand
    {
        Guid Id { get; }
        string Name { get; }
        string? Description { get; }
        bool IsActive { get; }
    }
}
