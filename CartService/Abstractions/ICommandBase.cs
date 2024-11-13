using FluentResults;
using MediatR;

namespace UserService.Abstractions
{
    public interface ICommandBase : IRequest<Result>
    {
    }
}
