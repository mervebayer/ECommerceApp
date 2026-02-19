using AutoMapper;
using ECommerceApp.Core.DTOs.Categories;
using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.DTOs.Users;
using ECommerceApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Mappings
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
        }
    }
}
