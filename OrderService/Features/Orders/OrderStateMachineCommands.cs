using OrderService.Models;

namespace OrderService.Features.Orders
{
    public record ReserveInventory(Guid OrderId, CartItem[] Items);
    
    public record CompleteOrder(Guid OrderId);
    public record NotifyOrderFailed(Guid OrderId, string Reason);
    public record NotifyOrderCancelled(Guid OrderId);
    public record NotifyOrderShipped(Guid OrderId);

}
