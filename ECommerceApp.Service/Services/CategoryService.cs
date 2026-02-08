using AutoMapper;
using ECommerceApp.Core.DTOs.Categories;
using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Core.Interfaces.Repositories;
using ECommerceApp.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Category> _category;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IGenericRepository<Category> category, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _category = category;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var data = await _category.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(data);
        }

        public async Task<CategoryDto> AddAsync(CategoryCreateDto entity)
        {
            var data = _mapper.Map<Category>(entity);
            await _category.AddAsync(data);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CategoryDto>(data);
        }

        
    }
}
