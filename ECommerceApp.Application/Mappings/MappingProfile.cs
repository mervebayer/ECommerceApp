using AutoMapper;
using ECommerceApp.Application.DTOs.Categories;
using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.DTOs.Users;
using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
            //CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductCreateDto, ProductDto>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryCreateDto, Category>().ReverseMap();
            CreateMap<CategoryUpdateDto, Category>();
            CreateMap<UserRegisterDto, AppUser>();
            CreateMap<AppUser, UserDto>();
            CreateMap<ProductImage, ProductImageDto>();

            // NOTE: AutoMapper uses constructor mapping for positional records. 
            // Since OrderListDto is a positional record, use ForCtorParam instead of ForMember.

            CreateMap<Order, OrderListDto>()
                .ForCtorParam(nameof(OrderListDto.OrderId), opt => opt.MapFrom(src => src.Id))
                .ForCtorParam(nameof(OrderListDto.TotalAmount), opt => opt.MapFrom(src => src.TotalAmount))
                .ForCtorParam(nameof(OrderListDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
                .ForCtorParam(nameof(OrderListDto.ItemCount), opt => opt.MapFrom(src => src.Items.Count))
                .ForCtorParam(nameof(OrderListDto.CreatedDate), opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<OrderItem, OrderItemDto>()
                .ForCtorParam(nameof(OrderItemDto.ProductId), opt => opt.MapFrom(src => src.ProductId))
                .ForCtorParam(nameof(OrderItemDto.ProductName), opt => opt.MapFrom(src => src.ProductName))
                .ForCtorParam(nameof(OrderItemDto.UnitPrice), opt => opt.MapFrom(src => src.UnitPrice))
                .ForCtorParam(nameof(OrderItemDto.Quantity), opt => opt.MapFrom(src => src.Quantity))
                .ForCtorParam(nameof(OrderItemDto.LineTotal), opt => opt.MapFrom(src => src.LineTotal));

            CreateMap<Order, OrderDetailDto>()
                .ForCtorParam(nameof(OrderDetailDto.OrderId), opt => opt.MapFrom(src => src.Id))
                .ForCtorParam(nameof(OrderDetailDto.TotalAmount), opt => opt.MapFrom(src => src.TotalAmount))
                .ForCtorParam(nameof(OrderDetailDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
                .ForCtorParam(nameof(OrderDetailDto.CreatedDate), opt => opt.MapFrom(src => src.CreatedDate))
                .ForCtorParam(nameof(OrderDetailDto.Items), opt => opt.MapFrom(src => src.Items));
        }
    }
}
