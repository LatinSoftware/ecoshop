namespace OrderService.Contracts;
public record InventoryReserved(Guid OrderId);
public record InventoryReservationFailed(Guid OrderId, string Reason);
public record PaymentAuthorizationFailed(Guid OrderId, string Reason);
public record OrderCompleted(Guid OrderId);
public record OrderCancelled(Guid OrderId, string Reason);
public record OrderShipped(Guid OrderId, string TrackingNumber);
public record OrderDelivered(Guid OrderId, DateTime DeliveredAt);
public record ShippingFailed(Guid OrderId, string Reason);
