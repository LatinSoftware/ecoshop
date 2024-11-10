using FluentAssertions;
using FluentValidation.TestHelper;
using ProductService.Entities;
using ProductService.Features.Products.Create;
using Xunit;

namespace ProductService.Features.Products.Update
{
    public sealed class ProductUpdateTest(ProductFixture productFixture) : IClassFixture<ProductFixture>
    {
        [Fact]
        public async Task UpdateProduct_WithNameOnly_ShouldUpdateNameSuccessfully()
        {
            //arrange
            var products = await productFixture.GetProducts();
            var productToUpdate = products.First();

            var productNameToUpdate = "producto #2";
            var command = new ProductUpdateCommand
            {
                ProductId = productToUpdate.Id.Value,
                Name = productNameToUpdate
            };

            //act
            var result = await productFixture.sender.Send(command, default);

            // assert

            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            var updatedProduct = await productFixture.GetProductById(productToUpdate.Id);

            updatedProduct.Should().NotBeNull();
            updatedProduct!.Name.Should().ContainEquivalentOf(productNameToUpdate);
            updatedProduct!.Price.Should().Be(productToUpdate.Price);
            updatedProduct!.Description.Should().Be(productToUpdate.Description);
            updatedProduct!.CategoryId.Should().Be(productToUpdate.CategoryId);

        }

        [Fact]
        public async Task UpdateProduct_WithPriceOnly_ShouldUpdatePriceSuccessfully()
        {
            //arrange
            var products = await productFixture.GetProducts();
            var productToUpdate = products.First();

            var newProductPrice = 30;
            var command = new ProductUpdateCommand
            {
                ProductId = productToUpdate.Id.Value,
                Price = newProductPrice
            };

            //act
            var result = await productFixture.sender.Send(command, default);

            // assert

            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            var updatedProduct = await productFixture.GetProductById(productToUpdate.Id);

            updatedProduct.Should().NotBeNull();
            updatedProduct!.Name.Should().Be(productToUpdate.Name);
            updatedProduct!.Price.Should().Be(newProductPrice);
            updatedProduct!.Description.Should().Be(productToUpdate.Description);
            updatedProduct!.CategoryId.Should().Be(productToUpdate.CategoryId);

        }

        [Fact]
        public async Task UpdateProduct_WithDescriptionOnly_ShouldUpdateDescriptionSuccessfully()
        {
            //arrange
            var products = await productFixture.GetProducts();
            var productToUpdate = products.First();

            var newProductDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ligula mauris, posuere in ante ut, rutrum auctor dui. Pellentesque sed sagittis neque. Morbi id pulvinar sem. In id diam ac diam venenatis dapibus a id leo. Phasellus vel nibh ut arcu porttitor auctor in ac tortor. Nulla egestas mi volutpat nunc elementum, nec scelerisque ligula eleifend.";
            var command = new ProductUpdateCommand
            {
                ProductId = productToUpdate.Id.Value,
                Description = newProductDescription
            };

            //act
            var result = await productFixture.sender.Send(command, default);

            // assert

            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            var updatedProduct = await productFixture.GetProductById(productToUpdate.Id);

            updatedProduct.Should().NotBeNull();
            updatedProduct!.Name.Should().Be(productToUpdate.Name);
            updatedProduct!.Price.Should().Be(productToUpdate.Price);
            updatedProduct!.Description.Should().Be(newProductDescription);
            updatedProduct!.CategoryId.Should().Be(productToUpdate.CategoryId);

        }

        [Fact]
        public async Task UpdateProduct_WithAllFields_ShouldUpdateAllFieldsSuccessfully()
        {
            //arrange
            var products = await productFixture.GetProducts();
            var productToUpdate = products.First();

            var command = new ProductUpdateCommand
            {
                ProductId = productToUpdate.Id.Value,
                Name= "product #2",
                Price = 25.30m,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ligula mauris, posuere in ante ut, rutrum auctor dui. Pellentesque sed sagittis neque. Morbi id pulvinar sem. In id diam ac diam venenatis dapibus a id leo. Phasellus vel nibh ut arcu porttitor auctor in ac tortor. Nulla egestas mi volutpat nunc elementum, nec scelerisque ligula eleifend.",
            };

            //act
            var result = await productFixture.sender.Send(command, default);

            // assert

            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            var updatedProduct = await productFixture.GetProductById(productToUpdate.Id);

            updatedProduct.Should().NotBeNull();
            updatedProduct!.Name.Should().ContainEquivalentOf(command.Name);
            updatedProduct!.Price.Should().Be(command.Price);
            updatedProduct!.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task UpdateProduct_WithoutProductId_ShouldReturnValidationError()
        {
            //arrange
            var products = await productFixture.GetProducts();
            var productToUpdate = products.First();

            var command = new ProductUpdateCommand
            {
                Name = "product #2",
                Price = 25.30m,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ligula mauris, posuere in ante ut, rutrum auctor dui. Pellentesque sed sagittis neque. Morbi id pulvinar sem. In id diam ac diam venenatis dapibus a id leo. Phasellus vel nibh ut arcu porttitor auctor in ac tortor. Nulla egestas mi volutpat nunc elementum, nec scelerisque ligula eleifend.",
            };

            //act
            var result = await productFixture.sender.Send(command, default);

            // assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();

            var validator = new ProductUpdateValidator();
            var validatorResult = validator.TestValidate(command);
            validatorResult.ShouldHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public async Task UpdateProduct_WithInvalidProductId_ShouldReturnNotFoundError()
        {
            
            //arrange
            var command = new ProductUpdateCommand
            {
                ProductId = Guid.NewGuid(),
                Name = "product #2",
                Price = 25.30m,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ligula mauris, posuere in ante ut, rutrum auctor dui. Pellentesque sed sagittis neque. Morbi id pulvinar sem. In id diam ac diam venenatis dapibus a id leo. Phasellus vel nibh ut arcu porttitor auctor in ac tortor. Nulla egestas mi volutpat nunc elementum, nec scelerisque ligula eleifend.",
            };

            //act
            var result = await productFixture.sender.Send(command, default);

            // assert

            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();

            var messages = result.Errors.Select(x => x.Message);
            messages.Should().HaveCount(1);
            messages.Should().Contain(ProductErrors.NotFound(ProductId.Create(command.ProductId)).Message);

        }
    }
}
