namespace OrderService.Abstractions
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
