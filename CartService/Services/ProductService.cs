using CartService.Abstractions.Services;
using CartService.Models;
using FluentResults;

namespace CartService.Services
{
    public class ProductService(IHttpClientFactory factory) : IProductService
    {
        public async Task<Result<ProductModel>> GetAsync(Guid id)
        {

            try
            {
                using var client = factory.CreateClient("product");
                var response = await client.GetFromJsonAsync<ProductModel>($"{id}");
                return Result.Ok(response!);
            }
            catch (Exception ex)
            {

                return new Error(ex.Message).CausedBy(ex);
            }
            //var resultTask = Result.Try(() => httpClient.GetFromJsonAsync<ProductModel>($"{basePath}/{id}"),           
            //    ex => new Error(ex.Message).CausedBy(ex));

            //return await resultTask;
        }
    }
}
