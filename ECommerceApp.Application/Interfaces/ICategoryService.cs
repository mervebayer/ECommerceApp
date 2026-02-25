using ECommerceApp.Application.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
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
