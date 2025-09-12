using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies.ValueObjects
{
	public sealed class PhoneType : ValueObject
	{
		public string Value { get; }

		// Constructor sin parámetros para EF Core
		private PhoneType() => Value = string.Empty;

        private PhoneType(string value) => Value = value;

        public static PhoneType Create(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("PhoneType is required", nameof(value));
			var normalized = value.Trim();
			if (normalized.Length > 50) throw new ArgumentException("PhoneType length cannot exceed 50 characters", nameof(value));
			
			// Validar que sea uno de los tipos permitidos
			var allowedTypes = new[] { "Principal", "Móvil", "Fax", "WhatsApp" };
			if (!allowedTypes.Contains(normalized))
			{
				throw new ArgumentException($"PhoneType must be one of: {string.Join(", ", allowedTypes)}", nameof(value));
			}
			
			return new PhoneType(normalized);
		}

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return Value;
		}

		public override string ToString() => Value;
	}
}
