using CartService.Entities;
using CartService.Errors;
using CartService.Features.Carts;
using CartService.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using MongoDB.Driver;

namespace CartService.Test.CartTest
{
    public class CartCreateTest : BaseIntegrationTest
    {

        private readonly IMongoCollection<Cart> collection;

        public CartCreateTest(IntegrationTestWebAppFactory factory) : base(factory)
        {
            collection = Database.GetCollection<Cart>(nameof(Cart));
        }

        [Fact]
        public async Task CartCreate_ShouldCreateEmptyCart_WhenCreatedWithoutItems()
        {
            //arrange
            var userId = Guid.NewGuid();
            var items = new List<CartItemRequest>();
            var command = new CartCreate.Command(userId, items);

            //act
            var result = await Sender.Send(command);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task CartCreate_ShouldCreateNewCartWithItem_WhenUserDoesNotHaveCart()
        {
            //arrange
            var userId = Guid.NewGuid();
            var items = new List<CartItemRequest>(){
                new () { ProductId = Guid.NewGuid(), Price = 250m, Quantity = 2 },
                new () { ProductId = Guid.NewGuid(), Price = 350.99m, Quantity = 2 },
            };
            var command = new CartCreate.Command(userId, items);

            //act
            var result = await Sender.Send(command);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task CartCreate_ShouldNotCreateDuplicateCart_WhenCartAlreadyExists()
        {
            //arrange
            var userId = Guid.NewGuid();
            await CreateCart(userId);
            var items = new List<CartItemRequest>(){
                new () { ProductId = Guid.NewGuid(), Price = 250m, Quantity = 2 },
                new () { ProductId = Guid.NewGuid(), Price = 350.99m, Quantity = 2 },
            };
            var command = new CartCreate.Command(userId, items);

            //act
            var result = await Sender.Send(command);

            //assert
            result.IsSuccess.Should().BeTrue();
            var carts = await GetUserCart(userId);
            carts.Should().HaveCount(1);
            carts.Select(c => c.Id).Should().ContainSingle(result.Value.Id);
            
        }

        [Fact]
        public async Task CartCreate_ShouldReturnFailure_WhenUserInputIsInvalid()
        {
            //arrange
            var userId = Guid.Empty;
            var items = new List<CartItemRequest>(){
                new () { ProductId = Guid.NewGuid(), Price = 250m, Quantity = 2 },
                new () { ProductId = Guid.NewGuid(), Price = 350.99m, Quantity = 2 },
            };
            var command = new CartCreate.Command(userId, items);

            //act
            var result = await Sender.Send(command);

            //assert
            result.IsSuccess.Should().BeFalse();
            var validator = new CartCreate.CartValidator();
            var validatorResult =  await validator.TestValidateAsync(command);
            validatorResult.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        private async Task CreateCart(Guid userId)
        {
            var cart = Cart.Create(userId);
            await CartRepository.CreateAsync(cart);
        }

        private async Task<ICollection<Cart>> GetUserCart(Guid userId)
        {
            return await collection.Find(c => c.UserId == userId).ToListAsync();
        }

    }
}
