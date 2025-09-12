using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies
{
	public class AddressTypeEntity : Entity
	{
		public string Name { get; private set; } = string.Empty;
		public string? Description { get; private set; }
		public bool IsActive { get; private set; }

		private AddressTypeEntity() { }

		private AddressTypeEntity(string name, string? description = null)
		{
			Id = Guid.NewGuid();
			Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
			Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
			IsActive = true;
		}

		public static AddressTypeEntity Create(string name, string? description = null)
			=> new AddressTypeEntity(name, description);

		public void UpdateInfo(string name, string? description = null)
		{
			Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
			Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
		}

		public void Activate() => IsActive = true;
		public void Deactivate() => IsActive = false;
	}
}
