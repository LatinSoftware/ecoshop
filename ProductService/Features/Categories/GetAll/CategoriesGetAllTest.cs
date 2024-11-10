using FluentAssertions;
using Xunit;

namespace ProductService.Features.Categories.GetAll
{
    public class CategoriesGetAllTest(CategoryFixture fixture) : IClassFixture<CategoryFixture>, IDisposable
    {

        [Fact]
        public async Task CategoryGetAll_ShouldReturnEmpty_WhenNoCategoryIsRegistred()
        {
            // arrange

            var command = new CategoryGetAllCommand();
            var handler = new CategoryGetAllCommandHandler(fixture.context, fixture.mapper);
            // act

            var result = await handler.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
            
        }

        [Fact]
        public async Task CategoryGetAll_ShouldReturnAllCategories_WhenThereAreCategories()
        {
            // arrange
            
            var categoriesDataset = await fixture.GetCategories();

            var command = new CategoryGetAllCommand();
            var handler = new CategoryGetAllCommandHandler(fixture.context, fixture.mapper);

            // act
            var result = await handler.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty()
                .And.HaveCount(categoriesDataset.Count)
                .And.OnlyHaveUniqueItems();

        }

        public void Dispose()
        {
            fixture.ClearDatabase();
        }
    }
}
