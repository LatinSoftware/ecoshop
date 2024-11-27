using FluentResults;
using FluentValidation;
using MediatR;
using ProductService.Exceptions;

namespace ProductService.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse: ResultBase, new()
       
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {

            if (!_validators.Any())
            {
                return await next();
            }

            var validationResult = await ValidateAsync(request);
            if(validationResult.IsFailed)
            {
                var result = new TResponse();
                foreach (var reason in validationResult.Reasons)
                {
                    result.Reasons.Add(reason);
                }

                return result;
            }

            return await next();

            
        }

       
        private async Task<Result> ValidateAsync(TRequest request)
        {
            ApplicationError[] errors = _validators.Select(validator => validator.Validate(request))
                .SelectMany(validationResult => validationResult.Errors)
                .Where(validationFailure => validationFailure is not null)
                .Select(failure => new ApplicationError(failure.PropertyName, failure.ErrorMessage))
                .Distinct()
                .ToArray();

            await Task.CompletedTask;
            if (errors.Length == 0)
                return Result.Ok();

            return Result.Fail(errors);
        }
    }
}
