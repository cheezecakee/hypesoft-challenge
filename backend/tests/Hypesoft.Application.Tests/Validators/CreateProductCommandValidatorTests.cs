using FluentAssertions;
using FluentValidation.TestHelper;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.Validators;
using Xunit;

namespace Hypesoft.Application.Tests.Validators
{
    public class CreateProductCommandValidatorTests
    {
        private readonly CreateProductCommandValidator _validator;

        public CreateProductCommandValidatorTests()
        {
            _validator = new CreateProductCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var command = new CreateProductCommand("", "Desc", 10m, "USD", "cat1", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Name);

            command = new CreateProductCommand(null!, "Desc", 10m, "USD", "cat1", 5);
            result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_MaxLength()
        {
            var longName = new string('A', 201);
            var command = new CreateProductCommand(longName, "Desc", 10m, "USD", "cat1", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var command = new CreateProductCommand("Product", "", 10m, "USD", "cat1", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_NonPositive()
        {
            var command = new CreateProductCommand("Product", "Desc", 0m, "USD", "cat1", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Price);
        }

        [Fact]
        public void Should_Have_Error_When_Currency_Is_Invalid()
        {
            var command = new CreateProductCommand("Product", "Desc", 10m, "", "cat1", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Currency);

            command = new CreateProductCommand("Product", "Desc", 10m, "US", "cat1", 5);
            result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Currency);
        }

        [Fact]
        public void Should_Have_Error_When_CategoryId_Is_Empty()
        {
            var command = new CreateProductCommand("Product", "Desc", 10m, "USD", "", 5);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.CategoryId);
        }

        [Fact]
        public void Should_Have_Error_When_StockQuantity_Is_Negative()
        {
            var command = new CreateProductCommand("Product", "Desc", 10m, "USD", "cat1", -1);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.StockQuantity);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            var command = new CreateProductCommand("Product", "Desc", 10m, "USD", "cat1", 5);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
