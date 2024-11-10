using FluentAssertions;
using ProductService.Entities;
using Xunit;

namespace ProductService.Features.Products.Delete
{
    public sealed class ProductDeleteTest(ProductFixture productFixture) : IClassFixture<ProductFixture>
    {
        [Fact]
        public async Task ProductDelete_WithExistingId_ShouldDeleteTheProduct()
        {
           
            //arrange
            var product = await productFixture.GetProduct();
            var command = new ProductDeleteCommand(product!.Id);
            //act
            var result = await productFixture.sender.Send(command);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            var deleteProduct = await productFixture.GetProductById(product.Id);

            deleteProduct.Should().BeNull();
        }

        [Fact]
        public async Task ProductDelete_WithNonExistentId_ShouldReturnNotFoundError()
        {

            //arrange
            var productId = ProductId.Create(Guid.NewGuid());
            var command = new ProductDeleteCommand(productId);
            //act
            var result = await productFixture.sender.Send(command);

            //assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();

            result.Errors.Should().HaveCount(1);
            result.Errors.Should().ContainEquivalentOf(ProductErrors.NotFound(productId));
        }
    }
}
