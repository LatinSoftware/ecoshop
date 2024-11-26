using Bogus;
using CartService.Entities;
using CartService.Errors;
using CartService.Features.CartItems;
using CartService.Models;
using FluentAssertions;
using MongoDB.Driver;

namespace CartService.Test.CartTest
{
    public class CartItemUpdateQuantityTest : BaseIntegrationTest
    {
        private readonly IMongoCollection<Cart> collection;
        public CartItemUpdateQuantityTest(IntegrationTestWebAppFactory factory) : base(factory)
        {
            collection = Database.GetCollection<Cart>(nameof(Cart));
        }

        [Fact]
        public async Task CartItemUpdateQuantity_ShouldUpdateQuantity_WhenItemExistsInCart()
        {
            //arrange
            var cart = await CreateCartWithItems();
            var itemToUpdate = cart.Items.First();
            var newQuantity = new Faker().Random.Number(1, 100);
            var command = new CartItemUpdateQuantity.Command(cart.Id!, itemToUpdate.Id!, newQuantity);

            //act
            var result = await Sender.Send(command);

            // assert
            result.IsSuccess.Should().BeTrue();

            cart = await GetCartById(cart.Id!);
            var updatedCartItem = cart.Items.First();
            updatedCartItem.Quantity.Should().Be(newQuantity);
        }

        [Fact]
        public async Task CartItemUpdateQuantity_ShouldThrowException_WhenItemDoesNotExist()
        {
            //arrange
            var cart = await CreateCartWithItems();
            var itemId = Guid.NewGuid().ToString();
            var newQuantity = new Faker().Random.Number(1, 100);
            var command = new CartItemUpdateQuantity.Command(cart.Id!, itemId, newQuantity);

            //act
            var result = await Sender.Send(command);

            // assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().ContainEquivalentOf(ItemErrorMessage.NotFound(itemId));

        }

        private async Task<Cart> CreateCartWithItems()
        {
            var items = new List<CartItemRequest>()
            {
                new ()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                },
                new ()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                },
                new ()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                },
            };
            var cart = await Sender.Send(new Features.Carts.CartCreate.Command(Guid.NewGuid(), items));
            return cart.Value;
        }

        private Task<Cart> GetCartById(string id) => collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    }


}
