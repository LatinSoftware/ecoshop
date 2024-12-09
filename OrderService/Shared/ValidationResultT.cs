using FluentResults;
using OrderService.Abstractions;
using OrderService.Errors;

namespace OrderService.Shared
{
    public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
    {
        private ValidationResult(ApplicationError[] errors)
        {
            Errors = errors;
        }

        public ApplicationError[] Errors { get; }

        public static ValidationResult<TValue> WithErrors(ApplicationError[] errors) => new(errors);
    }
}
