using System.Text.RegularExpressions;
using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies.ValueObjects
{
	public sealed class TaxId : ValueObject
	{
		public string Value { get; }

        private TaxId(string value) => Value = value;

        public static TaxId Create(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("TaxId is required", nameof(value));
			var normalized = Regex.Replace(value, "[^A-Za-z0-9]", string.Empty).ToUpperInvariant();
			if (normalized.Length < 6 || normalized.Length > 20)
			{
				throw new ArgumentException("TaxId length is invalid", nameof(value));
			}
			return new TaxId(normalized);
		}

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return Value;
		}

		public override string ToString() => Value;
	}
}
