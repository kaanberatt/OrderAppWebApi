using AutoMapper;
using OrderAppWebApi.Models.Dtos;
using OrderAppWebApi.Models.Entites;

namespace OrderAppWebApi.Context
{
    //Auto-Mapper
    public class MapperProfile: Profile
    { 
        public MapperProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<CreateOrderRequest, Order>();
            CreateMap<Order, CreateOrderRequest>();
            CreateMap<ProductDetailDto , OrderDetail>();
            CreateMap<OrderDetail, ProductDetailDto>();
        }
    }
}
