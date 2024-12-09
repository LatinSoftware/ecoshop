using OrderService.Models;
using Refit;

namespace OrderService.Abstractions
{
    public interface ICartApi
    {
        [Get("/carts/{userId}")]
        Task<CartModel> GetByUserId(Guid userId);
    }
}
