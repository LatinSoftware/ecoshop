using FluentAssertions;
using Xunit;

namespace ProductService.Features.Products.Filter
{
    public class ProductFilterTest(ProductFixture productFixture) : IClassFixture<ProductFixture>
    {

        [Fact]
        public async Task SearchProducts_WithoutFilters_ShouldReturnAllProducts()
        {
            // arrange
            var query = new ProductFilterQuery();
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            var products = await productFixture.GetProducts();
            result.Value.Should().HaveCount(products.Count);

        }

        [Fact]
        public async Task SearchProducts_ByExactName_ShouldReturnMatchingProducts()
        {
            // arrange
            var query = new ProductFilterQuery
            {
                Name = "Product #1"
            };
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            result.Value.Should().NotBeEmpty();
            result.Value.Select(p => p.Name).ToList().Should().ContainSingle(query.Name);

        }

        [Fact]
        public async Task SearchProducts_ByPartialName_ShouldReturnMatchingProducts()
        {
            // arrange
            var query = new ProductFilterQuery
            {
                Name = "Product"
            };
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            result.Value.Should().NotBeEmpty();
            
            var expectedProductsName =  productFixture.context.Products.Select(p => p.Name).Where(name => name.ToLower().Contains(query.Name.ToLower()));

            result.Value.Select(p => p.Name).ToArray().Should().Equal(expectedProductsName);
            result.Value.Count.Should().Be(expectedProductsName.Count());
        }

        [Fact]
        public async Task SearchProducts_ByPriceRange_ShouldReturnProductsWithinRange()
        {
            // arrange
            var query = new ProductFilterQuery
            {
               MinPrice = 2.1m,
               MaxPrice = 10.5m,
            };
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            result.Value.Should().NotBeEmpty();

            var expectedProducts = productFixture.context.Products
                .Where(product => product.Price >= query.MinPrice && product.Price <= query.MaxPrice);

            result.Value.Count.Should().Be(expectedProducts.Count());
        }

        [Fact]
        public async Task SearchProducts_ByPartialDescription_ShouldReturnMatchingProducts()
        {
            // arrange
            var query = new ProductFilterQuery
            {
                Description = "description"
            };
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            result.Value.Should().NotBeEmpty();

            var expectedProducts = productFixture.context.Products
                .Where(product => product.Description.ToLower().Contains(query.Description));

            result.Value.Count.Should().Be(expectedProducts.Count());
            result.Value.Select(product => product.Description).ToList().Should().ContainInOrder(expectedProducts.Select(p => p.Description));
        }

        [Fact]
        public async Task SearchProducts_ByMultipleFilters_ShouldReturnMatchingProducts()
        {
            // arrange
            var query = new ProductFilterQuery
            {
                Name = "product",
                MinPrice = 15m,
                MaxPrice = 40m,
            };
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            result.Value.Should().NotBeEmpty();

            var expectedProducts = productFixture.context.Products
                .Where(product => product.Price >= query.MinPrice && product.Price <= query.MaxPrice && product.Name.ToLower().Contains(query.Name.ToLower()));

            result.Value.Count.Should().Be(expectedProducts.Count());
        }

        [Fact]
        public async Task SearchProducts_ByCategory_ShouldReturnProductsInCategory()
        {
            var categories = await productFixture.GetCategories();
            var category = categories.First();
            // arrange
            var query = new ProductFilterQuery
            {
                CategoryId = category.Id.Value
            };
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();

            result.Value.Should().NotBeEmpty();

            var expectedProducts = productFixture.context.Products
                .Where(product => product.CategoryId == category.Id);

            result.Value.Count.Should().Be(expectedProducts.Count());
            result.Value.Select(p => p.Name).ToList().Should().ContainInOrder(expectedProducts.Select(p => p.Name));

        }

        [Fact]
        public async Task SearchProducts_WithNoMatchingResults_ShouldReturnEmptyList()
        {

            // arrange
            var query = new ProductFilterQuery
            {
                Name = "salami",
                MinPrice = 50.25m
            };
            // act
            var result = await productFixture.sender.Send(query);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.Value.Should().BeEmpty();
        }


    }
}
