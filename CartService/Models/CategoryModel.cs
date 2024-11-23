namespace CartService.Models
{
    public record CategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
