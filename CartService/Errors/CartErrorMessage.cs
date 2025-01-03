﻿
namespace CartService.Errors
{
    public static class CartErrorMessage
    {
        public static ApplicationError NotFoundForUser(Guid id) => new("Cart.NotFound.User", $"Cart for user with id '{id}' Not found or was deleted");
        public static ApplicationError NotFound(string id) => new("Cart.NotFound", $"Cart with id '{id}' Not found or was deleted");

        public static ApplicationError UserIsEmpty(Guid id) => new ("User.IsEmpty", $"User with id ${id} should not be empty");
        public static ApplicationError UserMisMatch { get; } = new("User.Mismatch", "User  given do not match with the identity user");
       
    }
}
