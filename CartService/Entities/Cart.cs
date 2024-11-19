using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CartService.Entities
{
    public class Cart
    {
        private List<CartItem> items { get; set; } = [];

        private Cart() { }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; private set; }
        public Guid? UserId { get; private set; }
        public decimal Total { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public List<CartItem> Items
        {
            get => items;
            private set => items = value ?? [];
        }

        public static Cart Create(Guid UserId)
        {
            return new Cart
            {
                UserId = UserId,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public void AddItem(CartItem item)
        {
            items.Add(item);
            Total = items.Sum(x => x.Total);
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeleteItem(CartItem item)
        {
            if (!items.Contains(item)) return;

            items.Remove(item);
        }
    }

    
}
