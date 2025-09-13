using FluentAssertions;
using FluentValidation.TestHelper;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.Validators;
using Xunit;

namespace Hypesoft.Application.Tests.Validators
{
    public class UpdateCategoryCommandValidatorTests
    {
        private readonly UpdateCategoryCommandValidator _validator;

        public UpdateCategoryCommandValidatorTests()
        {
            _validator = new UpdateCategoryCommandValidator();
        }

        [Theory]
        [InlineData(null, true)]   // Name not provided → valid
        [InlineData("", false)]    // Empty string → invalid
        [InlineData("Valid Name", true)]
        public void Should_Validate_Name_Correctly(string? name, bool isValid)
        {
            var command = new UpdateCategoryCommand("cat1", Name: name);
            var result = _validator.TestValidate(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Name);
            else
                result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("Valid description", true)]
        public void Should_Validate_Description_Correctly(string? description, bool isValid)
        {
            var command = new UpdateCategoryCommand("cat1", Description: description);
            var result = _validator.TestValidate(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Description);
            else
                result.ShouldHaveValidationErrorFor(c => c.Description);
        }


        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_MaxLength()
        {
            string longDescription = new string('x', 501); // 501 chars
            var command = new UpdateCategoryCommand("cat1", Description: longDescription);
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Description);
        }
    }
}
