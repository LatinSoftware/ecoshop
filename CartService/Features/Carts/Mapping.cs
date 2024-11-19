using AutoMapper;
using CartService.Entities;
using CartService.Models;

namespace CartService.Features.Carts
{
    
    public sealed class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Cart, CartModel>();
            CreateMap<CartItem, CartItemResponse>();
        }
    }
    
}
