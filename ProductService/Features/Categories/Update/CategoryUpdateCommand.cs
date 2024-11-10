using FluentResults;
using MediatR;
using Newtonsoft.Json;
using ProductService.Entities;

namespace ProductService.Features.Categories.Update
{
    public class CategoryUpdateCommand : IRequest<Result>
    {
        public CategoryUpdateCommand()
        {
            
        }
        public CategoryUpdateCommand(CategoryId categoryId, string name)
        {
            CategoryId = categoryId;
            Name = name;
        }

        [JsonIgnore]
        public CategoryId? CategoryId { get; set; } 
        public string Name { get; set; } = string.Empty;
    }

}
