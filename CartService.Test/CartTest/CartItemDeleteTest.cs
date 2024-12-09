using CartService.Entities;
using CartService.Errors;
using CartService.Features.CartItems;
using CartService.Models;
using FluentAssertions;
using MongoDB.Driver;

namespace CartService.Test.CartTest
{
    public class CartItemDeleteTest : BaseIntegrationTest
    {

        private readonly IMongoCollection<Cart> collection;

        public CartItemDeleteTest(IntegrationTestWebAppFactory factory) : base(factory)
        {
            collection = Database.GetCollection<Cart>(nameof(Cart));
        }

        [Fact]
        public async Task CartItemDelete_ShouldRemoveItem_WhenItemExistsInCart()
        {
            //arrange
            var cart = await CreateCartWithItems();
            var itemToDelete = cart.Items.First();

            var command = new CartItemDelete.Command(cart.Id!, itemToDelete.Id!);

            //act
            var result = await Sender.Send(command);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            cart = await GetCartById(cart.Id!);

            cart.Items.Should().HaveCount(2);
            cart.Items.Should().NotContain(itemToDelete);
        }

        [Fact]
        public async Task CartItemDelete_ShouldNotModifyCart_WhenItemDoesNotExist()
        {
            //arrange
            var cart = await CreateCartWithItems();

            var command = new CartItemDelete.Command(cart.Id!, Guid.NewGuid().ToString());

            //act
            var result = await Sender.Send(command);

            //assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            cart = await GetCartById(cart.Id!);

            cart.Items.Should().HaveCount(cart.Items.Count);
           
        }

        [Fact]
        public async Task CartItemDelete_ShouldReturnFailure_WhenCartIsNull()
        {
            //arrange
            var cart = await CreateCartWithItems();
            var itemToDelete = cart.Items.First();
            var cartId = "647f1d9e0c98b9a1dcba7c34";
            var command = new CartItemDelete.Command(cartId, itemToDelete.Id!);

            //act
            var result = await Sender.Send(command);

            //assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().ContainEquivalentOf(CartErrorMessage.NotFound(cartId));

       
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
                    Quantity = 1,
                },
            };
            var cart = await Sender.Send(new Features.Carts.CartCreate.Command(Guid.NewGuid(), items));
            return cart.Value;
        }

        private Task<Cart> GetCartById(string id) => collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    }
}
