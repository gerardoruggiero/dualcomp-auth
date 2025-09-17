using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
    /// <summary>
    /// Entidad base para tipos que contienen Name, Description e IsActive
    /// </summary>
    public abstract class BaseTypeEntity : Entity
    {
        public string Name { get; protected set; } = string.Empty;
        public string? Description { get; protected set; }
        public bool IsActive { get; protected set; }

        protected BaseTypeEntity() { }

        protected BaseTypeEntity(string name, string? description = null)
        {
            Id = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            IsActive = true;
        }

        public virtual void UpdateInfo(string name, string? description = null)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        public virtual void UpdateInfo(string name, string? description = null, bool isActive = true)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            IsActive = isActive;
        }

        public virtual void Activate() => IsActive = true;
        public virtual void Deactivate() => IsActive = false;
    }
}
