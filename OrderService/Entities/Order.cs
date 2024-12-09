using FluentResults;
using OrderService.Errors;

namespace OrderService.Entities
{
    public class Order
    {
        public Order()
        {
            Id = new OrderId(Guid.NewGuid());
        }
        public OrderId Id { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal Taxes { get; private set; }
        public decimal Total => Subtotal + Taxes;
        public OrderStatus Status { get; private set; } = OrderStatus.PENDING;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; private set; } = DateTime.UtcNow;

        private readonly List<OrderItem> _items = [];
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public PaymentInfo? PaymentInfo { get; private set; }


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


        public Result SetPaymentInfo(PaymentInfo paymentInfo)
        {
            if (paymentInfo == null)
                return Result.Fail($"Resource name {nameof(PaymentInfo)} cannot be null");
            PaymentInfo = paymentInfo;
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
