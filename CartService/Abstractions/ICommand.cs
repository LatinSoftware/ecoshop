using FluentResults;
using MediatR;

namespace CartService.Abstractions
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {

    }
}
