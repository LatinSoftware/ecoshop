using CartService.Abstractions.Repositories;
using CartService.Entities;
using CartService.Errors;
using FluentResults;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CartService.Database.Repositories
{
    public class CartRepository(IMongoDatabase mongoDatabase) : ICartRepository
    {
        private readonly IMongoCollection<Cart> _collection = mongoDatabase.GetCollection<Cart>(nameof(Cart));

        public async Task<List<Cart>> GetAsync(Expression<Func<Cart, bool>> filter) => await _collection.Find(filter).ToListAsync();
        public async Task<Result<Cart>> GetAsync(string id)
        {
            var cart = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (cart == null) return Result.Fail(CartErrorMessage.NotFound(id));
            return Result.Ok(cart);
        }
        public async Task<Result<Cart>> GetByUserId(Guid id)
        {
            var entity = await _collection.Find(x => x.UserId == id).FirstOrDefaultAsync();
            if (entity == null)
                return Result.Fail(CartErrorMessage.NotFoundForUser(id));
            return Result.Ok(entity);
        }
        public async Task CreateAsync(Cart newCart) => await _collection.InsertOneAsync(newCart);
        public async Task UpdateAsync(string id, Cart updatedCart) => await _collection.ReplaceOneAsync(x => x.Id == id, updatedCart);
        public async Task RemoveAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);

    }
}
