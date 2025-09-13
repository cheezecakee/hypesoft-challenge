using Hypesoft.Domain.ValueObjects;

namespace Hypesoft.Domain.Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Constructor_ValidValues_ShouldCreateMoney()
        {
            // Arrange
            decimal amount = 100.50m;
            string currency = "USD";

            // Act
            var money = new Money(amount, currency);

            // Assert
            money.Amount.Should().Be(amount);
            money.Currency.Should().Be("USD");
        }

        [Fact]
        public void Constructor_NegativeAmount_ShouldThrowException()
        {
            // Arrange
            decimal negativeAmount = -10.00m;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Money(negativeAmount));
            exception.Message.Should().Contain("Amount cannot be negative");
        }

        [Fact]
        public void Constructor_EmptyCurrency_ShouldThrowException()
        {
            // Arrange
            decimal amount = 100.00m;
            string emptyCurrency = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Money(amount, emptyCurrency));
            exception.Message.Should().Contain("Currency cannot be empty");
        }

        [Fact]
        public void Constructor_LowercaseCurrency_ShouldConvertToUppercase()
        {
            // Arrange
            decimal amount = 100.00m;
            string lowercaseCurrency = "usd";

            // Act
            var money = new Money(amount, lowercaseCurrency);

            // Assert
            money.Currency.Should().Be("USD");
        }

        [Fact]
        public void Add_SameCurrency_ShouldReturnCorrectSum()
        {
            // Arrange
            var money1 = new Money(100.00m, "USD");
            var money2 = new Money(50.50m, "USD");

            // Act
            var result = money1.Add(money2);

            // Assert
            result.Amount.Should().Be(150.50m);
            result.Currency.Should().Be("USD");
        }

        [Fact]
        public void Add_DifferentCurrencies_ShouldThrowException()
        {
            // Arrange
            var usdMoney = new Money(100.00m, "USD");
            var eurMoney = new Money(50.00m, "EUR");

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => usdMoney.Add(eurMoney));
            exception.Message.Should().Contain("Cannot add different currencies");
        }

        [Fact]
        public void Subtract_SameCurrency_ShouldReturnCorrectDifference()
        {
            // Arrange
            var money1 = new Money(100.00m, "USD");
            var money2 = new Money(30.50m, "USD");

            // Act
            var result = money1.Subtract(money2);

            // Assert
            result.Amount.Should().Be(69.50m);
            result.Currency.Should().Be("USD");
        }

        [Fact]
        public void Subtract_ResultingNegative_ShouldThrowException()
        {
            // Arrange
            var money1 = new Money(50.00m, "USD");
            var money2 = new Money(100.00m, "USD");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => money1.Subtract(money2));
            exception.Message.Should().Contain("Amount cannot be negative");
        }

        [Theory]
        [InlineData(100.00, "USD", 100.00, "USD", true)]
        [InlineData(100.00, "USD", 100.00, "EUR", false)]
        [InlineData(100.00, "USD", 50.00, "USD", false)]
        public void Equals_VariousScenarios_ShouldReturnExpectedResult(
            decimal amount1, string currency1, decimal amount2, string currency2, bool expected)
        {
            // Arrange
            var money1 = new Money(amount1, currency1);
            var money2 = new Money(amount2, currency2);

            // Act
            var result = money1.Equals(money2);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var money = new Money(100.50m, "USD");

            // Act
            var result = money.ToString();

            // Assert
            result.Should().Be("100.50 USD");
        }
    }
}
