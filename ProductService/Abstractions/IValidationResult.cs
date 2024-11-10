using ProductService.Extensions;

namespace ProductService.Abstractions
{
    public interface IValidationResult
    {
        public static readonly ApplicationError ValidationError = new ("ValidationError", "A validation problem occurred.");

        ApplicationError[] Errors { get; }
    }
}
