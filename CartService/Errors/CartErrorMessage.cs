namespace CartService.Errors
{
    public static class CartErrorMessage
    {
        public static ApplicationError NotFound(Guid id) => new("Cart.NotFound", $"Cart with id '{id}' Not found or was deleted");
    }
}
