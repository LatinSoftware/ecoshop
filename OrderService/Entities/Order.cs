using FluentResults;
using OrderService.Errors;

namespace OrderService.Entities
{
    public class Order
    {

        private Order(UserId userId)
        {
            OrderId = new OrderId(Guid.NewGuid());
            UserId = userId;
            UpdateTimestamp();
        }

        public OrderId OrderId { get; private set; }
        public UserId UserId { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal Taxes { get; private set; }
        public decimal Total => Subtotal + Taxes;
        public OrderStatus Status { get; private set; } = OrderStatus.PENDING;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; private set; } = DateTime.UtcNow;
        private readonly List<OrderItem> _items = [];
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public Payment? Payment { get; private set; }


        public static Order Create(UserId userId)
        {
            return new Order(userId);
        }

        public Result AddItem(OrderItem item)
        {
            if (item == null) return Result.Fail("Item cannot be null");
            _items.Add(item);
            RecalculateSubtotal();
            UpdateTimestamp();
            return Result.Ok();
        }

        public Result RemoveItem(Guid itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);

            if (item == null) return Result.Fail(OrderErrorMessage.ItemNotFound);
            _items.Remove(item);
            RecalculateSubtotal();
            UpdateTimestamp();

            return Result.Ok();
        }

        public Result UpdateStatus(OrderStatus status)
        {
            if (status == Status)
                return Result.Fail("Order is already in the desired status.");

            Status = status;
            UpdateTimestamp();
            return Result.Ok();
        }

        public Result SetPaymentInfo(Payment paymentInfo)
        {
            if (paymentInfo == null)
                return Result.Fail($"Resource name {nameof(Payment)} cannot be null");
            //Payment = paymentInfo;
            UpdateTimestamp();
            return Result.Ok();
        }

        private void RecalculateSubtotal()
        {
            Subtotal = _items.Sum(item => item.Total.Value);
            Taxes = Subtotal * 0.1m;
        }

        private void UpdateTimestamp()
        {
            LastUpdatedAt = DateTime.UtcNow;
        }

    }

    public enum OrderStatus
    {
        PENDING,
        PAID,
        SHIPPED,
        DELIVERED,
        CANCELLED
    }
}
