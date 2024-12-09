using FluentResults;
using ProductService.Abstractions;
using ProductService.Exceptions;

namespace ProductService.Shared
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
