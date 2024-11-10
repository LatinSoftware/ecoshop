using FluentResults;

namespace ProductService.Extensions
{
    public static class ResultExtension
    {
        public static T Match<T>(this Result<T> result, Func<T> onSuccess, Func<List<IError>, T> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result.Errors);
        }
    }
}
