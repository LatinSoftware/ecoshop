namespace CartService.Errors
{
    public static class ItemErrorMessage
    {
        public static ApplicationError NotFound(string id) => new("Item.NotFound", $"Item with id `{id}` not found");
    }
}
