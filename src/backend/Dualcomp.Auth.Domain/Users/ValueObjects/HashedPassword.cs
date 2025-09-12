using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Users.ValueObjects
{
    public class HashedPassword : ValueObject
    {
        public string Value { get; private set; }

        private HashedPassword() => Value = string.Empty;

        private HashedPassword(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Hashed password cannot be null or empty", nameof(value));

            Value = value;
        }

        public static HashedPassword Create(string value) => new HashedPassword(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
