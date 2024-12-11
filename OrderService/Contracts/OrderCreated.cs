using OrderService.Entities;
using OrderService.Models;

namespace OrderService.Contracts;

public record OrderCreated(Guid OrderId, Guid UserId, decimal TotalAmount, OrderItem[] Items);
