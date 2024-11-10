namespace ProductService.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string? Description { get; private set; }
        public CategoryModel? Category { get; private set; }
    }
}
