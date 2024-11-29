using FluentResults;
using MediatR;

namespace OrderService.Abstractions
{
    public interface ICommandBase : IRequest<Result>
    {
    }
}
