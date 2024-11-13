namespace CartService.Models
{
    public class CartItemResponse
    {
        public string Id { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
