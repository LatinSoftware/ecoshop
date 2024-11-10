using FluentResults;
using MediatR;
using ProductService.Entities;

namespace ProductService.Features.Categories.Delete
{
    public record CategoryDeleteCommand(CategoryId CategoryId) : IRequest<Result>;
}
