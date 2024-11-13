using FluentResults;
using MediatR;

namespace CartService.Abstractions
{
    public interface ICommandBase : IRequest<Result>
    {
    }
}
