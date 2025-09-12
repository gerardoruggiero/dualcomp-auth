using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies.ValueObjects
{
	public sealed class SocialMediaType : ValueObject
	{
		public string Value { get; }

		// Constructor sin parámetros para EF Core
		private SocialMediaType() => Value = string.Empty;

        private SocialMediaType(string value) => Value = value;

        public static SocialMediaType Create(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("SocialMediaType is required", nameof(value));
			var normalized = value.Trim();
			if (normalized.Length > 50) throw new ArgumentException("SocialMediaType length cannot exceed 50 characters", nameof(value));
			
			// Validar que sea uno de los tipos permitidos
			var allowedTypes = new[] { "Facebook", "Instagram", "LinkedIn", "Twitter", "YouTube" };
			if (!allowedTypes.Contains(normalized))
			{
				throw new ArgumentException($"SocialMediaType must be one of: {string.Join(", ", allowedTypes)}", nameof(value));
			}
			
			return new SocialMediaType(normalized);
		}

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return Value;
		}

		public override string ToString() => Value;
	}
}
