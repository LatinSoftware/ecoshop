using FluentResults;
using MediatR;

namespace ProductService.Abstractions
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse>: IRequest<Result<TResponse>>
    {

    }
}
