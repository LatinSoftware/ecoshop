using FluentResults;
using MediatR;
using ProductService.Models;

namespace ProductService.Features.Categories.Create
{
    public class CategoryCreateCommand(string name) : IRequest<Result<CategoryModel>>
    {
        public string Name { get; set; } = name;
    }
}
