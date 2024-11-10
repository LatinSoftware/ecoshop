using FluentResults;
using MediatR;

namespace ProductService.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
