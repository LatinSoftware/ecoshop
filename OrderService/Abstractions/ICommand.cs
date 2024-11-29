using FluentResults;


namespace OrderService.Abstractions
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {

    }
}
