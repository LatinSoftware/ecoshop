using AutoMapper;
using ProductService.Entities;
using ProductService.Models;

namespace ProductService.Features.Products
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(product => product.Id.Value));
                
        }
    }
}
