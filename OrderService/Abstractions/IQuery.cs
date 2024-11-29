using FluentResults;
using MediatR;

namespace OrderService.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
