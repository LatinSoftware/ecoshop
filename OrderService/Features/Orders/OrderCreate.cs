﻿using FluentResults;
using MassTransit;
using MassTransit.Mediator;
using OrderService.Abstractions;
using OrderService.Contracts;
using OrderService.Database;
using OrderService.Entities;
using OrderService.Models;

namespace OrderService.Features.Orders
{
    public sealed class OrderCreate
    {
        public record OrderCreatedResponse
        {
            public Guid Id { get;  set; }
            public Guid UserId { get; set; }
            public decimal Subtotal { get;  set; }
            public decimal Taxes { get;  set; }
            public decimal Total { get; set; }
            public OrderStatus Status { get;  set; }
            public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        }
        public record Command : Request<Result<OrderCreatedResponse>>
        {
            public string CartId { get; set; } = string.Empty;
            public Guid UserId { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
        }

       

        public sealed class Handler(ICartApi cartApi, IProductApi productApi, ApplicationContext appContext, IBus bus) : MediatorRequestHandler<Command, Result<OrderCreatedResponse>>
        {
            protected override async Task<Result<OrderCreatedResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get user cart
                var cartResult = await cartApi.GetByUserId(request.UserId);

                // validate if cart is not empty

                if (cartResult.Items.Count == 0)
                    return Result.Fail("Cart do not have items!!!!");

                // validate products stock and price

                var productTask = cartResult.Items.Select(GetAvailableProduct);

                var productResponses = await Task.WhenAll(productTask);
                var productResult = Result.Merge(productResponses);

                if (productResult.IsFailed)
                    return Result.Fail(productResult.Errors);

                // create payment authorization



                // create order
                var order = new Order();

                cartResult.Items.ToList().ForEach(item => 
                {
                    var orderItemResult = OrderItem.Create(order.Id, new ProductId(item.ProductId), new Quantity(item.Quantity), new Money(item.Price));

                    if (orderItemResult.IsFailed)
                        return;

                    order.AddItem(orderItemResult.Value);
                });

                await appContext.Orders.AddAsync(order, cancellationToken);

                await appContext.SaveChangesAsync(cancellationToken);

                

                await bus.Publish(new OrderCreated(order.Id.Value, request.UserId, order.Total, [.. order.Items]), cancellationToken);


                return Result.Ok(new OrderCreatedResponse
                {
                    Id = order.Id.Value,
                    UserId = request.UserId,
                    Subtotal = order.Subtotal,
                    Taxes = order.Taxes,
                    Total = order.Total,
                    Status = order.Status,
                    Items = order.Items.ToList()
                });
            }

           
            private async Task<Result<ProductModel>> GetAvailableProduct(CartItem item)
            {
                var productResponse = await productApi.GetStock(item.ProductId);

                if(!productResponse.IsSuccessful)
                {
                    return Result.Fail($"Product with id: {item.ProductId} not found");
                }

                var product = productResponse.Content.Data!;

                var result = Result.Merge(
                    Result.FailIf(product.AvailableQuantity == 0,$"Product ${product.Name} is not available."),
                    Result.FailIf(product.Price != item.Price, $"Product ${product.Name} has an invalid price.")
                    );

               if(result.IsFailed) return result;

               return Result.Ok(product);
            }


        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("/orders", async (Command command, IMediator mediator) =>
                {
                    var result = await mediator.SendRequest(command);

                    if (result.IsSuccess)
                        return Results.Ok(result.Value);

                    return Results.Ok(result.Errors);
                });
            }
        }
    }
}