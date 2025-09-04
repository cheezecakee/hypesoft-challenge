using System.Globalization;

namespace Hypesoft.Domain.ValueObjects
{
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = string.Empty;

        private Money() { } // EF Core

        public Money(decimal amount, string currency = "USD")
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            }
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency cannot be empty", nameof(currency));
            }

            Amount = amount;
            Currency = currency.ToUpper(CultureInfo.InvariantCulture);
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
            {
                throw new InvalidOperationException("Cannot add different currencies");
            }
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
            {
                throw new InvalidOperationException("Cannot subtract different currencies");
            }

            return new Money(Amount - other.Amount, Currency);
        }

        public bool Equals(Money? other)
        {
            if (other is null)
            {
                return false;
            }
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Money);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        public static bool operator ==(Money? left, Money? right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(Money? left, Money? right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{Amount:F2} {Currency}";
        }
    }
}
