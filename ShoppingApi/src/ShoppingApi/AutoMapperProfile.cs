using AutoMapper;
using ShoppingApi.Shared.Models;
using ShoppingApi.ShoppingCart;

namespace ShoppingApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductDTO, Product>();
            CreateMap<Product, ProductDTO>();

            CreateMap<CartDTO, Cart>();
            CreateMap<Cart, CartDTO>();
        }
    }
}
