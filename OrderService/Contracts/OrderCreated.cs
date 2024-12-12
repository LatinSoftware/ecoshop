﻿using OrderService.Entities;

namespace OrderService.Contracts;

public record OrderCreated(Guid OrderId, Guid UserId, decimal TotalAmount, OrderItemCreated[] Items);

public record OrderItemCreated(Guid Id, OrderId OrderId, Quantity Quantity, Money Price);