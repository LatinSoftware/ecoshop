namespace ProductService.Shared
{
    public static class Constants
    {
        public static string AdminRole { get; } = "admin";

        public static class Category
        {
            public static string Tag { get; } = "Category";
            public static string CreateEndpointName { get; } = "CategoryCreate";
        }
    }
}
