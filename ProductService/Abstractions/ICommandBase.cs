using FluentResults;
using MediatR;

namespace ProductService.Abstractions
{
    public interface ICommandBase : IRequest<Result>
    {
    }
}
