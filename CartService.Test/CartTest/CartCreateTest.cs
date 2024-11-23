using CartService.Features.Carts;
using CartService.Models;
using FluentAssertions;

namespace CartService.Test.CartTest
{
    public class CartCreateTest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
    {
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
    }
}
