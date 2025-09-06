using FluentValidation;
using Hypesoft.Application.Commands.Products;

namespace Hypesoft.Application.Validators
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product ID is required");

            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Product name cannot be empty when provided")
                    .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");
            });

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Product description cannot be empty when provided");
            });

            When(x => x.Price.HasValue, () =>
            {
                RuleFor(x => x.Price)
                    .GreaterThan(0).WithMessage("Price must be greater than zero");
            });

            When(x => x.Currency != null, () =>
            {
                RuleFor(x => x.Currency)
                    .NotEmpty().WithMessage("Currency cannot be empty when provided")
                    .Length(3).WithMessage("Currency must be 3 characters long");
            });

            When(x => x.CategoryId != null, () =>
            {
                RuleFor(x => x.CategoryId)
                    .NotEmpty().WithMessage("Category ID cannot be empty when provided");
            });

            RuleFor(x => x)
                .Must(x => x.Name != null || x.Description != null ||
                          x.Price.HasValue || x.CategoryId != null)
                .WithMessage("At least one field must be provided for update");
        }
    }
}
