using CartService.Models;
using FluentResults;

namespace CartService.Abstractions.Services
{
    public interface IProductService
    {
        public Task<Result<ProductModel>> GetAsync(Guid Id);
    }
}
