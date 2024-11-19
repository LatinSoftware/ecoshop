﻿using CartService.Models;
using FluentValidation;

namespace CartService.Features.CartItems
{

    public sealed class CartItemValidator : AbstractValidator<CartItemRequest>
    {
        public CartItemValidator()
        {
            RuleFor(c => c.ProductId).NotNull().NotEmpty();
            RuleFor(c => c.Quantity).NotNull().GreaterThan(0);
            RuleFor(c => c.Price).NotNull().GreaterThanOrEqualTo(0).PrecisionScale(10, 2, false);
        }
    }

}