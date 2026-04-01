using AutoMapper;
using ECommerceApp.Application.DTOs.Addresses;
using ECommerceApp.Application.DTOs.Categories;
using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.Orders.Admin;
using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.DTOs.UserProfiles;
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

            CreateMap<UserAddress, UserAddressDto>();
            CreateMap<CreateUserAddressDto, UserAddress>();
            CreateMap<UpdateUserAddressDto, UserAddress>();
            CreateMap<AppUser, UserProfileDto>();

            // NOTE: AutoMapper uses constructor mapping for positional records. 
            // Since OrderListDto is a positional record, use ForCtorParam instead of ForMember.

            CreateMap<Order, OrderListDto>()
                .ForCtorParam(nameof(OrderListDto.OrderId), opt => opt.MapFrom(src => src.Id))
                .ForCtorParam(nameof(OrderListDto.OrderNumber), opt => opt.MapFrom(src => src.OrderNumber))
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
                .ForCtorParam(nameof(OrderDetailDto.OrderNumber), opt => opt.MapFrom(src => src.OrderNumber))
                .ForCtorParam(nameof(OrderDetailDto.TotalAmount), opt => opt.MapFrom(src => src.TotalAmount))
                .ForCtorParam(nameof(OrderDetailDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
                .ForCtorParam(nameof(OrderDetailDto.CreatedDate), opt => opt.MapFrom(src => src.CreatedDate))
                .ForCtorParam(nameof(OrderDetailDto.Items), opt => opt.MapFrom(src => src.Items))
                .ForCtorParam(nameof(OrderDetailDto.ShippingTitle), opt => opt.MapFrom(src => src.ShippingTitle))
                .ForCtorParam(nameof(OrderDetailDto.ShippingContactName), opt => opt.MapFrom(src => src.ShippingContactName))
                .ForCtorParam(nameof(OrderDetailDto.ShippingPhoneNumber), opt => opt.MapFrom(src => src.ShippingPhoneNumber))
                .ForCtorParam(nameof(OrderDetailDto.ShippingCountry), opt => opt.MapFrom(src => src.ShippingCountry))
                .ForCtorParam(nameof(OrderDetailDto.ShippingCity), opt => opt.MapFrom(src => src.ShippingCity))
                .ForCtorParam(nameof(OrderDetailDto.ShippingDistrict), opt => opt.MapFrom(src => src.ShippingDistrict))
                .ForCtorParam(nameof(OrderDetailDto.ShippingPostalCode), opt => opt.MapFrom(src => src.ShippingPostalCode))
                .ForCtorParam(nameof(OrderDetailDto.ShippingAddressLine), opt => opt.MapFrom(src => src.ShippingAddressLine));

            CreateMap<Order, AdminOrderListDto>()
                    .ForCtorParam(nameof(AdminOrderListDto.OrderId), opt => opt.MapFrom(src => src.Id))
                    .ForCtorParam(nameof(AdminOrderListDto.OrderNumber), opt => opt.MapFrom(src => src.OrderNumber))
                    .ForCtorParam(nameof(AdminOrderListDto.UserId), opt => opt.MapFrom(src => src.UserId))
                    .ForCtorParam(nameof(AdminOrderListDto.TotalAmount), opt => opt.MapFrom(src => src.TotalAmount))
                    .ForCtorParam(nameof(AdminOrderListDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
                    .ForCtorParam(nameof(AdminOrderListDto.ItemCount), opt => opt.MapFrom(src => src.Items.Count))
                    .ForCtorParam(nameof(AdminOrderListDto.CreatedDate), opt => opt.MapFrom(src => src.CreatedDate));


            CreateMap<Order, AdminOrderDetailDto>()
                .ForCtorParam(nameof(AdminOrderDetailDto.OrderId), opt => opt.MapFrom(src => src.Id))
                .ForCtorParam(nameof(AdminOrderDetailDto.UserId), opt => opt.MapFrom(src => src.UserId))
                .ForCtorParam(nameof(AdminOrderDetailDto.OrderNumber), opt => opt.MapFrom(src => src.OrderNumber))
                .ForCtorParam(nameof(AdminOrderDetailDto.TotalAmount), opt => opt.MapFrom(src => src.TotalAmount))
                .ForCtorParam(nameof(AdminOrderDetailDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
                .ForCtorParam(nameof(AdminOrderDetailDto.CreatedDate), opt => opt.MapFrom(src => src.CreatedDate))
                .ForCtorParam(nameof(AdminOrderDetailDto.Items), opt => opt.MapFrom(src => src.Items))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingTitle), opt => opt.MapFrom(src => src.ShippingTitle))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingContactName), opt => opt.MapFrom(src => src.ShippingContactName))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingPhoneNumber), opt => opt.MapFrom(src => src.ShippingPhoneNumber))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingCountry), opt => opt.MapFrom(src => src.ShippingCountry))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingCity), opt => opt.MapFrom(src => src.ShippingCity))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingDistrict), opt => opt.MapFrom(src => src.ShippingDistrict))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingPostalCode), opt => opt.MapFrom(src => src.ShippingPostalCode))
                .ForCtorParam(nameof(AdminOrderDetailDto.ShippingAddressLine), opt => opt.MapFrom(src => src.ShippingAddressLine));
        }
    }
}
