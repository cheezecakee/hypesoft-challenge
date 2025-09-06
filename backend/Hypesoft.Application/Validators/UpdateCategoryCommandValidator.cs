using FluentValidation;
using Hypesoft.Application.Commands.Categories;

namespace Hypesoft.Application.Validators
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
            {
                RuleFor(x => x.Name)
                    .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");
            });

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
            });

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Name) || x.Description != null)
                .WithMessage("At least one field must be provided for update");
        }
    }
}
