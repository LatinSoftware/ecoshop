namespace CartService.Entities
{
    public class CartItem
    {
        private CartItem() { }
        public string? Id { get; private set; } = string.Empty;
        public string? CartId { get; private set; }
        public Guid? ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        public decimal Total { get; private set; }

       

        public static CartItem Create(string CartId, Guid ProductId, int Quantity, decimal Price, decimal total)
        => new() { Id = Guid.NewGuid().ToString(), CartId = CartId, ProductId = ProductId, Quantity = Quantity, Price = Price, Total =  total};

        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
            Total = Price * Quantity;
        }
    }
}
