using CartService.Entities;
using CartService.Errors;
using CartService.Features.CartItems;
using CartService.Features.Carts;
using CartService.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using MongoDB.Driver;

namespace CartService.Test.CartTest
{
    public class CardAddItemTest : BaseIntegrationTest
    {
        private readonly IMongoCollection<Cart> collection;
        public CardAddItemTest(IntegrationTestWebAppFactory factory) : base(factory)
        {
            collection = Database.GetCollection<Cart>(nameof(Cart));
        }

        [Fact]
        public async Task CardAddItem_ShouldReturnFailure_WhenCartNotExist()
        {
            //arrange
            var cartId = "a1d34a13d46a1d64a123d4ad";
            var item = new CartItemRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1,
            };
            var command = new CartAddItem.Command(cartId, item);

            //act
            var result = await Sender.Send(command);

            // assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainEquivalentOf(CartErrorMessage.NotFound(cartId));
        }

        [Fact]
        public async Task CardAddItem_ShouldBeAddedToAnExistingCart_WhenCartItemIsValid()
        {
            //arrange
            var cart = await CreateCart();
            var item = new CartItemRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1,
            };
            var command = new CartAddItem.Command(cart.Id!, item);

            //act
            var result = await Sender.Send(command);

            // assert
            result.IsFailed.Should().BeFalse();
            result.IsSuccess.Should().BeTrue();

            var dbCart = await GetCartById(cart.Id!);
            dbCart.Should().NotBeNull();
            dbCart.Items.Should().HaveCountGreaterThan(0);
            dbCart.Items.Select(i => new CartItemRequest
            {
                ProductId = i.ProductId!.Value,
                Quantity = i.Quantity,
            }).Should().ContainEquivalentOf(item);
           
        }

        [Theory]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(5)]
        [InlineData(12)]
        public async Task CardAddItem_ShouldUpdateQuantity_WhenCartItemAlreadyExist(int quantity)
        {
            //arrange
            var cart = await CreateCartWithItems();
            var item = cart.Items.First();
            var itemToUpdate = new CartItemRequest
            {
                ProductId = item.ProductId.GetValueOrDefault(),
                Quantity = quantity,
            };
            var command = new CartAddItem.Command(cart.Id!, itemToUpdate);

            //act
            var result = await Sender.Send(command);

            // assert
            result.IsFailed.Should().BeFalse();
            result.IsSuccess.Should().BeTrue();

            var dbCart = await GetCartById(cart.Id!);
            dbCart.Should().NotBeNull();
            dbCart.Items.First().Quantity.Should().Be(itemToUpdate.Quantity + item.Quantity);

        }

        [Fact]
        public async Task CardAddItem_ShouldReturnFailure_WhenCartItemIsNotValid()
        {
            //arrange
            var cart = await CreateCart();
            var item = new CartItemRequest
            {
                ProductId = Guid.Empty,
                Quantity = 0,
            };
            var command = new CartAddItem.Command(cart.Id!, item);

            //act
            var result = await Sender.Send(command);

            // assert
            result.IsFailed.Should().BeTrue();

            var validator = new CartAddItem.Validator();
            var validatorResult = await validator.TestValidateAsync(command);
            validatorResult.ShouldHaveValidationErrorFor(x => x.Item.ProductId);
            validatorResult.ShouldHaveValidationErrorFor(x => x.Item.Quantity);

        }

        private async Task<Cart> CreateCart()
        {
           var cart = await Sender.Send(new CartCreate.Command(Guid.NewGuid(), []));
            return cart.Value;
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
            var cart = await Sender.Send(new CartCreate.Command(Guid.NewGuid(), items));
            return cart.Value;
        }

        private Task<Cart> GetCartById(string id) => collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}
