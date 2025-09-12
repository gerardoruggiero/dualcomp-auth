using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies.ValueObjects
{
	public sealed class EmailType : ValueObject
	{
		public string Value { get; }

		// Constructor sin parámetros para EF Core
		private EmailType() => Value = string.Empty;

        private EmailType(string value) => Value = value;

        public static EmailType Create(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("EmailType is required", nameof(value));
			var normalized = value.Trim();
			if (normalized.Length > 50) throw new ArgumentException("EmailType length cannot exceed 50 characters", nameof(value));
			
			// Validar que sea uno de los tipos permitidos
			var allowedTypes = new[] { "Principal", "Facturación", "Soporte", "Comercial" };
			if (!allowedTypes.Contains(normalized))
			{
				throw new ArgumentException($"EmailType must be one of: {string.Join(", ", allowedTypes)}", nameof(value));
			}
			
			return new EmailType(normalized);
		}

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return Value;
		}

		public override string ToString() => Value;
	}
}
