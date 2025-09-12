using DualComp.Infraestructure.Domain.Domain.Common;
using System.Text.RegularExpressions;

namespace Dualcomp.Auth.Domain.Companies.ValueObjects
{
	public sealed class Email : ValueObject
	{
		public string Value { get; }

        private Email(string value) => Value = value;

        public static Email Create(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email is required", nameof(value));
			var normalized = value.Trim().ToLowerInvariant();
			if (!Regex.IsMatch(normalized, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			{
				throw new ArgumentException("Email format is invalid", nameof(value));
			}
			return new Email(normalized);
		}

		protected override IEnumerable<object?> GetEqualityComponents()
		{
			yield return Value;
		}

		public override string ToString() => Value;
	}
}
