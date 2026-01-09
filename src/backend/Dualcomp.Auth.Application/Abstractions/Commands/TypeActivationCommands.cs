namespace Dualcomp.Auth.Application.Abstractions.Commands
{
    public abstract class ActivateTypeCommandBase : IActivateTypeCommand
    {
        public Guid Id { get; set; }
    }

    public abstract class DeactivateTypeCommandBase : IDeactivateTypeCommand
    {
        public Guid Id { get; set; }
    }
}
