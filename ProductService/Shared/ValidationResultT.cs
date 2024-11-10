using FluentResults;
using ProductService.Abstractions;
using ProductService.Extensions;

namespace ProductService.Shared
{
    public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
    {
        private ValidationResult(ApplicationError[] errors)
        {
            Errors = errors;
        }

        public ApplicationError[] Errors { get;  }

        public static ValidationResult<TValue> WithErrors(ApplicationError[] errors) => new(errors);
    }
}
