using FluentResults;
using MediatR;

namespace CartService.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
