using FluentResults;
using OrderService.Abstractions;
using UserService.Errors;

namespace OrderService.Shared
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
