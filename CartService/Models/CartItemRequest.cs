namespace CartService.Models
{
    public class CartItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

    }
}
