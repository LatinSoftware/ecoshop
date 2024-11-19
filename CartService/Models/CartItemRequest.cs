namespace CartService.Models
{
    public class CartItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
