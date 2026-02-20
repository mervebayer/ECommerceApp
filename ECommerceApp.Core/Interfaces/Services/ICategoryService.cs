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
        Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryDto> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<CategoryDto> AddAsync(CategoryCreateDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(long id, CategoryUpdateDto entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    }
}
