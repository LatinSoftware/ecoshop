using FluentAssertions;
using Xunit;

namespace ProductService.Features.Categories.Create
{
    public class CategoryCreateTest(CategoryFixture fixture) : IClassFixture<CategoryFixture>
    {
        [Fact]
        public async Task CreateCategory_ShouldAddCategoryToDatabase_WhenCategoryIsValid()
        {
            // arrange
            var categoryToCreate = "House";
            var command = new CategoryCreateCommand(categoryToCreate);
            var handler = new CategoryCreateCommandHandler(fixture.context, fixture.mapper);

            // act
            var result = await handler.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(categoryToCreate);
        }

        [Fact]
        public async Task CreateCategory_ShouldAssignGuidId_WhenCategoryIsCreated() 
        {
            // arrange
            var categoryToCreate = "Robotic";
            var command = new CategoryCreateCommand(categoryToCreate);
            var handler = new CategoryCreateCommandHandler(fixture.context, fixture.mapper);

            // act
            var result = await handler.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(categoryToCreate);
            result.Value.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnIsFailure_WhenCategoryNameIsEmpty()
        {
            // arrange
            var categoryToCreate = string.Empty;
            var command = new CategoryCreateCommand(categoryToCreate);
            var handler = new CategoryCreateCommandHandler(fixture.context, fixture.mapper);

            // act
            var result = await handler.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.HasError(e => e.Message.Equals(CategoryErrors.InvalidName.Message)).Should().BeTrue();
          
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnIsFailure_WhenCategoryNameAlreadyExists()
        {
            // arrange
            var categoriesData = await fixture.GetCategories();
            var categoryToCreate =categoriesData.First();
            var command = new CategoryCreateCommand(categoryToCreate.Name);
            var handler = new CategoryCreateCommandHandler(fixture.context, fixture.mapper);

            // act
            var result = await handler.Handle(command, default);

            // assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.HasError(e => e.Message.Equals(CategoryErrors.AlreadyExist.Message)).Should().BeTrue();
        }
     }
}
