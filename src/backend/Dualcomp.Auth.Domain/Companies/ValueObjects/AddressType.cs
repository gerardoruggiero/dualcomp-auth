using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies.ValueObjects
{
	public sealed class AddressType : ValueObject
	{
		public string Value { get; }

		// Constructor sin parámetros para EF Core
		private AddressType() => Value = string.Empty;

        private AddressType(string value) => Value = value;

        public static AddressType Create(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("AddressType is required", nameof(value));
			var normalized = value.Trim();
			if (normalized.Length > 50) throw new ArgumentException("AddressType length cannot exceed 50 characters", nameof(value));
			
			// Validar que sea uno de los tipos permitidos
			var allowedTypes = new[] { "Principal", "Sucursal", "Facturación", "Envío" };
			if (!allowedTypes.Contains(normalized))
			{
				throw new ArgumentException($"AddressType must be one of: {string.Join(", ", allowedTypes)}", nameof(value));
			}
			
			return new AddressType(normalized);
		}

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return Value;
		}

		public override string ToString() => Value;
	}
}
