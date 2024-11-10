namespace ProductService.Extensions
{
    public static class Capitalize
    {
        public static string ToCapitalize(this string word)
        {
            return word[..1].ToUpper() + word[1..].ToLower();
        }
    }
}
