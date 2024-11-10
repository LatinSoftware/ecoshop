using ProductService.Extensions;

namespace ProductService.Entities
{
    public class Product
    {
        private Product()
        {
            
        }
        public ProductId Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string? Description { get; private set; }
        public CategoryId CategoryId { get; private set; }
        public Category Category { get; private set; }

        public static Product Create(CategoryId categoryId, string name, decimal price, string? description)
        {
            return new Product {
                Id = new ProductId(Guid.NewGuid()),
                Name = name.ToCapitalize(), 
                Price = price, 
                Description = description,
                CategoryId = categoryId,
            };
        }

        public void SetName(string? name)
        {
            if(string.IsNullOrEmpty(name)) 
                return;

            Name = name.ToCapitalize();
        }

        public void SetPrice(decimal? price)
        {
            if(price.HasValue)
            {
                Price = price.Value;
            }
        }

        public void SetDescription(string? description)
        {
            if (string.IsNullOrEmpty(description))
                return;

            Description = description;
        }
    }

    public record ProductId (Guid Value)
    {
        public static ProductId Create(Guid value) => new(value);
    };
}
