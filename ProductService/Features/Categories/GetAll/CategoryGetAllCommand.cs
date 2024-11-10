using FluentResults;
using MediatR;
using ProductService.Models;

namespace ProductService.Features.Categories.GetAll
{
    public class CategoryGetAllCommand : IRequest<Result<ICollection<CategoryModel>>>
    {

    }
}
