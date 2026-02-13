using ECommerceApp.Core.DTOs.Categories;
using ECommerceApp.Core.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(long id);
        Task<CategoryDto> AddAsync(CategoryCreateDto entity);
        Task UpdateAsync(long id, CategoryUpdateDto entity);
        Task DeleteAsync(long id);
    }
}
