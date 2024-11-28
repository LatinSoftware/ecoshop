using FluentResults;
using ProductService.Exceptions;
using ProductService.Shared;

namespace ProductService.Extensions
{
    public static class ResultExtension
    {
        public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, int successCode = 200, int errorCode = 400)
        {
            var response = new ApiResponse<T>()
            {
                Timestamp = DateTime.UtcNow,
            };

            if(result.IsSuccess)
            {
                response.Code = successCode;
                response.Data = result.ValueOrDefault;
                response.Message = string.Join(", ", result.Successes.Select(x => x.Message));
            }
            else
            {
                response.Code = errorCode;
                response.Errors = result.Errors.Select(e =>
                {
                    var code = e.Metadata.GetValueOrDefault("code","");
                    return new ApiError
                    {
                        Code = code?.ToString(),
                        Detail = e.Message
                    };
                }).ToList();
                
            }

            return response;
        }

        public static ApiResponse ToApiResponse(this Result result, int successCode = 200, int errorCode = 400, string message = "")
        {
            var response = new ApiResponse()
            {
                Timestamp = DateTime.UtcNow,
            };

            if (result.IsSuccess)
            {
                response.Code = successCode;
                response.Message = message;
            }
            else
            {
                response.Code = errorCode;
                response.Errors = result.Errors.Select(e =>
                {
                    var code = e.Metadata.GetValueOrDefault("code", "");
                    return new ApiError
                    {
                        Code = code?.ToString(),
                        Detail = e.Message
                    };
                }).ToList();

            }

            return response;
        }

        public static TResponse Match<TRequest, TResponse>(this Result<TRequest> result, Func<TResponse> onSuccess, Func<List<IError>, TResponse> onError)
        {
            return result.IsSuccess ? onSuccess() : onError(result.Errors);
        }

        public static TResponse Match<TResponse>(this Result result, Func<TResponse> onSuccess, Func<List<IError>, TResponse> onError)
        {
            return result.IsSuccess ? onSuccess() : onError(result.Errors);
        }

        public static bool HasErrorWithCode(this List<IError> errors, string code)
        {
            return errors.OfType<ApplicationError>().Any(e => e.Code == code);
        }
    }
}
