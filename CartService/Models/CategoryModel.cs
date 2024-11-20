namespace CartService.Models
{
    public record CategoryModel
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
    }
}
