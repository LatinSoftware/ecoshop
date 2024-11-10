using FluentAssertions;
using ProductService.Entities;
using Xunit;

namespace ProductService.Features.Categories.GetById
{
    public class CategoryGetByIdTest(CategoryFixture fixture) : IClassFixture<CategoryFixture>, IDisposable
    {
        [Fact]
        public async Task CategoryGetById_ShouldReturnCategory_WhenCategoryIdExist()
        {
            // arrange
            var categoryData = await fixture.GetCategory();
            var command = new CategoryGetByIdCommand(categoryData.Id);
            var handler = new CategoryGetByIdCommandHandler(fixture.context, fixture.mapper);

            // act
            var category = await handler.Handle(command, default);

            // assert
            category.IsSuccess.Should().BeTrue();
            category.Value.Should().NotBeNull();
            category.Value.Name.Should().Be(categoryData.Name);
            category.Value.Id.Should().Be(categoryData.Id.Value);
        }

        [Fact]
        public async Task CategoryGetById_ShouldReturnErrorAsTrue_WhenIdNotExist()
        {
            // arrange
            var categoryData = await fixture.GetCategory();
            var categoryId = new CategoryId(Guid.NewGuid());
            var command = new CategoryGetByIdCommand(categoryId);
            var handler = new CategoryGetByIdCommandHandler(fixture.context, fixture.mapper);

            // act
            var result = await handler.Handle(command, default);

            // assert

            result.IsFailed.Should().BeTrue();
            result.HasError(x => x.Message == CategoryErrors.NotFound(categoryId).Message).Should().BeTrue();
        }

        [Fact]
        public async Task CategoryGetById_ShouldHandleMultipleCategories()
        {
            // arrange

            var categoryData = await fixture.GetCategories();
            var category = categoryData[1];
            var command = new CategoryGetByIdCommand(category.Id);
            var handler = new CategoryGetByIdCommandHandler(fixture.context, fixture.mapper);

            // act
            var result = await handler.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.Value.Name.Should().Be(category.Name);
            result.Value.Id.Should().Be(category.Id.Value);
        }

        public void Dispose()
        {
            fixture.ClearDatabase();
        }
    }
}
