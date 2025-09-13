using FluentValidation;
using Hypesoft.Application.Commands.Categories;

namespace Hypesoft.Application.Validators
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            // Validate Name if provided (not null), including empty string
            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Category name cannot be empty")
                    .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");
            });

            // Validate Description if provided
            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
            });

            // Ensure at least one field is provided for update
            RuleFor(x => x)
                .Must(x => x.Name != null || x.Description != null)
                .WithMessage("At least one field must be provided for update");
        }
    }
}

