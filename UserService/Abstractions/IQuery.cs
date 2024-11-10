using FluentResults;
using MediatR;

namespace UserService.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
