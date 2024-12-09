namespace OrderService.Models
{
    public class CartModel
    {
        public string Id { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
