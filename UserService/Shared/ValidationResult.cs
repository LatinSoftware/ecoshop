using FluentResults;
using UserService.Abstractions;
using UserService.Errors;

namespace UserService.Shared
{
    public sealed class ValidationResult : Result, IValidationResult
    {
        private ValidationResult(ApplicationError[] errors)
        {
            Errors = errors;
        }

        public ApplicationError[] Errors { get; private set; }

        public static ValidationResult WithErrors(ApplicationError[] errors) => new(errors);
    }
}
