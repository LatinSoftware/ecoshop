namespace ProductService.Entities
{
    public class Category
    {
        private List<Product> _products = new();
        private Category()
        {
            
        }

        public CategoryId Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public static Category Create( string name) => new()
        { Name = name, Id = new CategoryId(Guid.NewGuid()) };

        public void SetName(string name) => Name = name;

        public IReadOnlyCollection<Product> Products => _products.ToList();
    }


    public record CategoryId(Guid Value);
}
