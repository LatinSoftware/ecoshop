using FluentAssertions;
using ProductService.Entities;
using ProductService.Exceptions;
using Xunit;

namespace ProductService.Features.Categories.Delete
{
    public class CategoryDeleteTest(CategoryFixture fixture) : IClassFixture<CategoryFixture>
    {
        [Fact]
        public async Task DeleteCategory_ShouldRemoveCategory_WhenCategoryExists()
        {
            // Arrange: Prepara el contexto y agrega una categoría que se pueda eliminar.
            var categoriesData = await fixture.GetCategories();
            var category = categoriesData.First();
            var command = new CategoryDeleteCommand(category.Id);
            var handler = new CategoryDeleteCommandHandler(fixture.context);

            // Act: Llama al método `DeleteCategory` con el Id de la categoría existente.
            var result = await handler.Handle(command, default);

            // Assert: Verifica que la categoría ya no esté presente en la base de datos.
            result.IsSuccess.Should().BeTrue();

            categoriesData = [.. fixture.context.Categories];
            categoriesData.Should().NotContain(category);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnIsFailure_WhenCategoryDoesNotExist()
        {
            // Arrange: Prepara el contexto sin agregar la categoría a eliminar.
            var categoryId = new CategoryId(Guid.NewGuid());
            var command = new CategoryDeleteCommand(categoryId);
            var handler = new CategoryDeleteCommandHandler(fixture.context);

            // Act: Llama al método `DeleteCategory` con un Id que no existe.
            var result = await handler.Handle(command, default);

            // Assert: Verifica que IsFailure es true y lanze not found message.
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.HasError<ApplicationError>(error => error.Message.Equals(CategoryErrors.NotFound(categoryId).Message));
        }
    }
}
