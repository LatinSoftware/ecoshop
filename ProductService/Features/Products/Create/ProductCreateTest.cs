using FluentAssertions;
using FluentValidation.TestHelper;
using ProductService.Entities;
using ProductService.Exceptions;
using ProductService.Features.Categories;
using Xunit;

namespace ProductService.Features.Products.Create
{
    public class ProductCreateTest(ProductFixture fixture) : IClassFixture<ProductFixture>
    {
        [Fact]
        public async Task CreateProduct_ShouldAddProductToDatabase_WhenProductIsValid()
        {
            // Arrange: Prepara el contexto y configura el servicio.

            var categories = await fixture.GetCategories();

            var category = categories.First();

            var command = new ProductCreateCommand()
            {
                Name = "Product #1",
                CategoryId = category.Id.Value,
                Price = 20.50m
            };


            // Act: Llama al método `CreateProduct` con un producto válido (nombre, descripción, precio, categoría, etc.).

            var result = await fixture.sender.Send(command, default);

            // Assert: Verifica que el producto se haya agregado a la base de datos con los datos correctos.

            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            result.Value.Name.Should().NotBeNullOrEmpty();
            result.Value.Name.Length.Should().BeGreaterThanOrEqualTo(5).And.BeLessThanOrEqualTo(500);

            result.Value.Price.Should().BeGreaterThan(0).And.BePositive();
            

            // validate that product returned is equal to the product in db
            var product = await fixture.GetProductById(new ProductId(result.Value.Id));

            product.Should().NotBeNull();
            product.Name.Should().Be(result.Value.Name);
            
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnFailure_WhenProductParamsAreInvalid()
        {
            // Arrange: Prepara el contexto y configura el servicio.

            var command = new ProductCreateCommand();
            // Act: Intenta crear un producto con un nombre `null` o vacío.

            var result = await fixture.sender.Send(command, default);

            // Assert: Verifica que el result sea nulo indicando las validaciones de los campos.
            result.IsFailed.Should().BeTrue();

            var validator = new ProductCreateCommandValidator();
            var validatorResult = validator.TestValidate(command);
            validatorResult.ShouldHaveValidationErrorFor(x => x.CategoryId);
            validatorResult.ShouldHaveValidationErrorFor(x => x.Name);
            validatorResult.ShouldHaveValidationErrorFor(x => x.Price);

        }

        [Fact]
        public async Task CreateProduct_ShouldReturnFailure_WhenProductCategoryNotExist()
        {
            var command = new ProductCreateCommand()
            {
                Name = "Product #2",
                CategoryId = Guid.NewGuid(),
                Price = 20.50m,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ligula mauris, posuere in ante ut, rutrum auctor dui. Pellentesque sed sagittis neque. Morbi id pulvinar sem. In id diam ac diam venenatis dapibus a id leo. Phasellus vel nibh ut arcu porttitor auctor in ac tortor. Nulla egestas mi volutpat nunc elementum, nec scelerisque ligula eleifend.",
            };

            var result = await fixture.sender.Send(command, default);

            result.IsFailed.Should().BeTrue();
            result.HasError<ApplicationError>().Should().BeTrue();

            var notFoundMessage = CategoryErrors.NotFound(new CategoryId(command.CategoryId));

            var errorMessages =  result.Errors.Select(e => e.Message).ToList();
            errorMessages.Should().HaveCount(1);
            errorMessages.Should().Contain(notFoundMessage.Message);
        }
    }
}
