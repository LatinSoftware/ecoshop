using FluentResults;
using MediatR;
using ProductService.Entities;
using ProductService.Models;

namespace ProductService.Features.Categories.GetById
{
    public class CategoryGetByIdCommand(CategoryId Id) : IRequest<Result<CategoryModel>>
    {
        public CategoryId Id { get; private set; } = Id;
    }
}