namespace CartService.Models
{
    public class CartModel
    {
        public string Id { get;  set; } = string.Empty;
        public Guid UserId { get; private set; }
        public decimal Total { get; private set; }
        public ICollection<CartItemResponse> Items { get; set; } = new HashSet<CartItemResponse>();
    }
}
