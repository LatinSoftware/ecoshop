using Bogus;
using CartService.Abstractions.Services;
using CartService.Models;
using FluentResults;

namespace CartService.Test.MockServices
{
    internal class ProductMockService : IProductService
    {
        public Task<Result<ProductModel>> GetAsync(Guid Id)
        {
            var faker = new Faker<ProductModel>()
                .RuleFor(p => p.Id, f => Id)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(100, 500, 2)))
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Category, f => new CategoryModel { Id = Guid.NewGuid(), Name = f.Commerce.Categories(1).First() });

            var product = faker.Generate();
            
            return Task.FromResult(Result.Ok(product)); 
                
        }
    }
}
