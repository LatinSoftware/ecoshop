using CartService.Entities;
using FluentResults;
using System.Linq.Expressions;

namespace CartService.Abstractions.Repositories
{
    public interface ICartRepository
    {
        Task CreateAsync(Cart newCart);
        Task<List<Cart>> GetAsync(Expression<Func<Cart, bool>> filter);
        Task<Result<Cart>> GetAsync(string id);
        Task<Result<Cart>> GetByUserId(Guid id);
        Task RemoveAsync(string id);
        Task UpdateAsync(string id, Cart updatedCart);
    }
}