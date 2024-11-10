using FluentAssertions;
using ProductService.Entities;
using Xunit;

namespace ProductService.Features.Categories.Update
{
    public class CategoryUpdateTest(CategoryFixture fixture) : IClassFixture<CategoryFixture>
    {
        [Fact]
        public async Task UpdateCategory_ShouldUpdateCategory_WhenCategoryExistsAndDataIsValid()
        {
            // Arrange: Prepara el contexto y agrega una categoría existente para actualizar.
            var categoryData = await fixture.GetCategory();
            var newCategoryName = "Cars";
            var command = new CategoryUpdateCommand(categoryData.Id, newCategoryName);
            var handler = new CategoryUpdateCommandHandler(fixture.context);

            // Act: Llama al método `UpdateCategory` con datos válidos para la actualización.
            var result = await handler.Handle(command, default);

            // Assert: Verifica que los cambios se hayan guardado en la base de datos.
            result.IsSuccess.Should().BeTrue();

            var categoryUpdated = await fixture.GetCategory(categoryData.Id);
            categoryUpdated.Should().NotBeNull();
            categoryUpdated?.Id.Should().Be(categoryData.Id);
            categoryUpdated?.Name.Should().Be(newCategoryName);

        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNotFoundMessage_WhenCategoryDoesNotExist()
        {
            // Arrange: Prepara el contexto sin agregar la categoría a actualizar.
            var categoryDataId =  new CategoryId(Guid.NewGuid());
            var newCategoryName = "Cars";
            var command = new CategoryUpdateCommand(categoryDataId, newCategoryName);
            var handler = new CategoryUpdateCommandHandler(fixture.context);

            // Act: Llama al método `UpdateCategory` con un Id que no existe.
            var result = await handler.Handle(command, default);

            // Assert: Verifica que se el result isFailed and return a notfound message.
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.Errors.Select(x => x.Message).Should().Contain(CategoryErrors.NotFound(categoryDataId).Message);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnIsFailure_WhenCategoryNameAlreadyExists()
        {
            // Arrange: Prepara el contexto con dos categorías, de las cuales una tiene el nombre que se usará para la actualización.
            var categoryData = await fixture.GetCategories();
            var categoryToUpdate = categoryData.First();
            var categoryNameAlreadyExist = categoryData[1].Name.ToLower();
            var command = new CategoryUpdateCommand(categoryToUpdate.Id, categoryNameAlreadyExist);
            var handler = new CategoryUpdateCommandHandler(fixture.context);

            // Act: Intenta actualizar una categoría usando un nombre que ya está en uso por otra categoría.
            var result = await handler.Handle(command, default);

            // Assert: Verifica el result.IsFailure is true y el mensaje indique que el nombre ya existe.
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.Errors.Select(x => x.Message).Should().Contain(CategoryErrors.AlreadyExist.Message);

        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnIsFailure_WhenCategoryNameIsNullOrEmpty()
        {
            // Arrange: Prepara el contexto y agrega una categoría existente para actualizar.
            var categoryData = await fixture.GetCategory();
            var newCategoryName = string.Empty;
            var command = new CategoryUpdateCommand(categoryData.Id, newCategoryName);
            var handler = new CategoryUpdateCommandHandler(fixture.context);

            // Act: Intenta actualizar la categoría con un nombre `null` o vacío.
            var result = await handler.Handle(command, default);

            // Assert: Verifica que se lance una excepción ArgumentException indicando que el nombre no puede ser nulo o vacío.
            result.IsSuccess.Should().BeFalse();
            result.Errors.Select(x => x.Message).Should().Contain(CategoryErrors.InvalidName.Message);

        }

        [Fact]
        public async Task UpdateCategory_ShouldNotChangeId_WhenUpdatingCategory()
        {
            // Arrange: Prepara el contexto y agrega una categoría existente para actualizar.
            var categoryData = await fixture.GetCategory();
            var newCategoryName = "Automation";
            var command = new CategoryUpdateCommand(categoryData.Id, newCategoryName);
            var handler = new CategoryUpdateCommandHandler(fixture.context);

            // Act: Llama al método `UpdateCategory` y modifica sólo el nombre de la categoría.
            var result = await handler.Handle(command, default);

            // Assert: Verifica que el `Id` de la categoría no haya cambiado.
            result.IsSuccess.Should().BeTrue();

            var categoryUpdated = await fixture.GetCategory(categoryData.Id);
            categoryUpdated.Should().NotBeNull();
            categoryUpdated?.Id.Should().Be(categoryData.Id);
            
        }

    }
}
