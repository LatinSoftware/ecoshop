using FluentResults;
using MediatR;

namespace UserService.Abstractions
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {

    }
}
