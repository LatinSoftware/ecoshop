using FluentResults;
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
        public StockQuantity StockQuantity { get; private set; } = new StockQuantity(0);
        public ReservedQuantity ReservedQuantity { get; private set; } = new ReservedQuantity(0);

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

        public void AdjustStock(int quantity)
        {
            var newStock = (int)StockQuantity + quantity;
            StockQuantity = new StockQuantity(newStock);
        }

        public Result ReserveStock(int quantity)
        {
            if (quantity > (int)StockQuantity - (int)ReservedQuantity)
                return Result.Fail("Not enough stock available.");

            ReservedQuantity = new ReservedQuantity((int)ReservedQuantity + quantity);
            return Result.Ok();
        }

        public Result ReleaseStock(int quantity)
        {
            if (quantity > (int)ReservedQuantity)
                Result.Fail("Cannot release more than reserved.");

            ReservedQuantity = new ReservedQuantity((int)ReservedQuantity - quantity);

            return Result.Ok();
        }
    }

    public record ProductId (Guid Value)
    {
        public static ProductId Create(Guid value) => new(value);
        public static implicit operator Guid(ProductId value) => value.Value;
        public static implicit operator ProductId(Guid value) => new (value);
    };

    public record StockQuantity
    {
        public int Value { get; init; }

        public StockQuantity(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Stock quantity cannot be negative.");
            Value = value;
        }

        public static implicit operator int(StockQuantity value) => value.Value;
        public static implicit operator StockQuantity(int value) => new (value);
        public override string ToString() => Value.ToString();
    }

    public record ReservedQuantity
    {
        public int Value { get; init; }

        public ReservedQuantity(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Stock reserved cannot be negative.");
            Value = value;
        }

        public static implicit operator int(ReservedQuantity value) => value.Value;
        public static implicit operator ReservedQuantity(int value) => new(value);
        public override string ToString() => Value.ToString();
    }
}
