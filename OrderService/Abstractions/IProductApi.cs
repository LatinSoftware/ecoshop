using OrderService.Models;
using Refit;

namespace OrderService.Abstractions
{
    public interface IProductApi
    {
        [Get("/products/{id}/availability")]
        public Task<ApiResponse<Shared.ApiResponse<ProductModel>>> GetStock(Guid id);
    }
}
