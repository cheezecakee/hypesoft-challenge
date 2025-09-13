using FluentAssertions;
using FluentValidation.TestHelper;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.Validators;
using Xunit;

namespace Hypesoft.Application.Tests.Validators
{
    public class CreateCategoryCommandValidatorTests
    {
        private readonly CreateCategoryCommandValidator _validator;

        public CreateCategoryCommandValidatorTests()
        {
            _validator = new CreateCategoryCommandValidator();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Valid Category", true)]
        public void Should_Validate_Name(string? name, bool isValid)
        {
            var command = new CreateCategoryCommand(name!, "Some description");
            var result = _validator.TestValidate(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Name);
            else
                result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("Some description", true)]
        public void Should_Validate_Description_NotNull(string? description, bool isValid)
        {
            var command = new CreateCategoryCommand("Valid Name", description!);
            var result = _validator.TestValidate(command);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Description);
            else
                result.ShouldHaveValidationErrorFor(c => c.Description);
        }
    }
}
