using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Users.ValueObjects
{
    public class Password : ValueObject
    {
        public string Value { get; private set; }

        private Password() => Value = string.Empty;

        private Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Password cannot be null or empty", nameof(value));

            Value = value;
        }

        public static Password Create(string value) => new Password(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
