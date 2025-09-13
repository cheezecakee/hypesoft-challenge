using FluentAssertions;
using FluentValidation.TestHelper;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.Validators;
using Xunit;

namespace Hypesoft.Application.Tests.Validators
{
    public class UpdateProductCommandValidatorTests
    {
        private readonly UpdateProductCommandValidator _validator;

        public UpdateProductCommandValidatorTests()
        {
            _validator = new UpdateProductCommandValidator();
        }

        [Theory]
        [InlineData(null, true)]    // Name not provided → valid
        [InlineData("", false)]     // Name empty string → invalid
        [InlineData("Valid Name", true)]
        public void Should_Validate_Name_Correctly(string? name, bool isValid)
        {
            var command = new UpdateProductCommand("prod1", Name: name);
            var result = _validator.TestValidate(command);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Name);
            else
                result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("Valid Description", true)]
        public void Should_Validate_Description_Correctly(string? description, bool isValid)
        {
            var command = new UpdateProductCommand("prod1", Description: description);
            var result = _validator.TestValidate(command);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Description);
            else
                result.ShouldHaveValidationErrorFor(c => c.Description);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(0.0, false)]
        [InlineData(-5.5, false)]
        [InlineData(10.0, true)]
        public void Should_Validate_Price_Correctly(double? priceDouble, bool isValid)
        {
            decimal? price = priceDouble.HasValue ? (decimal?)priceDouble.Value : null;
            var command = new UpdateProductCommand("prod1", Price: price);
            var result = _validator.TestValidate(command);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Price);
            else
                result.ShouldHaveValidationErrorFor(c => c.Price);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("US", false)]
        [InlineData("USD", true)]
        public void Should_Validate_Currency_Correctly(string? currency, bool isValid)
        {
            var command = new UpdateProductCommand("prod1", Currency: currency);
            var result = _validator.TestValidate(command);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Currency);
            else
                result.ShouldHaveValidationErrorFor(c => c.Currency);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("cat1", true)]
        public void Should_Validate_CategoryId_Correctly(string? categoryId, bool isValid)
        {
            var command = new UpdateProductCommand("prod1", CategoryId: categoryId);
            var result = _validator.TestValidate(command);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.CategoryId);
            else
                result.ShouldHaveValidationErrorFor(c => c.CategoryId);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(-1, false)]
        [InlineData(0, true)]
        [InlineData(10, true)]
        public void Should_Validate_StockQuantity_Correctly(int? stockQuantity, bool isValid)
        {
            var command = new UpdateProductCommand("prod1", StockQuantity: stockQuantity);
            var result = _validator.TestValidate(command);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.StockQuantity);
            else
                result.ShouldHaveValidationErrorFor(c => c.StockQuantity);
        }

        [Fact]
        public void Should_Have_Error_When_No_Fields_Provided()
        {
            var command = new UpdateProductCommand("prod1");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c);
        }
    }
}
