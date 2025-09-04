using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Hypesoft.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators
    ) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            if (validators.Any())
            {
                ValidationContext<TRequest> context = new(request);
                ValidationResult[] validationResults = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                List<ValidationFailure> failures = [.. validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)];

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next(cancellationToken);
        }
    }
}
