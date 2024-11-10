using FluentAssertions;
using ProductService.Entities;
using Xunit;

namespace ProductService.Features.Products.GetOne
{
    public sealed class ProductGetOneTest(ProductFixture productFixture) : IClassFixture<ProductFixture>
    {
        [Fact]
        public async Task ProductGetOne_WithValidId_ShouldReturnProduct()
        {
            //arrange
            var products = await productFixture.GetProducts();
            var product = products.First();
            var query = new ProductGetOneQuery(product.Id);

            //act
            var result = await productFixture.sender.Send(query);

            //assert
            result.IsFailed.Should().BeFalse();
            result.IsSuccess.Should().BeTrue();

            result.Value.Id.Should().Be(product.Id.Value);
            result.Value.Name.Should().Be(product.Name);
            result.Value.Price.Should().Be(product.Price);
            result.Value.Description.Should().Be(product.Description);
            result.Value.Category.Should().NotBeNull();
        }

        [Fact]
        public async Task GetProductById_WithNonExistentId_ShouldReturnNotFoundError()
        {
            //arrange
            var productId = ProductId.Create(Guid.NewGuid());
            var query = new ProductGetOneQuery(productId);

            //act
            var result = await productFixture.sender.Send(query);

            //assert
            result.IsFailed.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainEquivalentOf(ProductErrors.NotFound(productId));
        }
    }
}
