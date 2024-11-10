using FluentResults;
using UserService.Abstractions;
using UserService.Errors;

namespace UserService.Shared
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
