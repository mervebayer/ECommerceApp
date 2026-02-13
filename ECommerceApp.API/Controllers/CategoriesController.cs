using ECommerceApp.Core.DTOs.Categories;
using ECommerceApp.Core.Interfaces.Services;
using ECommerceApp.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _categoryService.GetAllAsync();
            return Ok(data);
        }
        [HttpGet("{id:long:min(1)}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _categoryService.GetByIdAsync(id);
            return Ok(data);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDto entity) { 
            var data = await _categoryService.AddAsync(entity);
            return StatusCode(201, data);
        }

        [HttpPut("{id:long:min(1)}")]
        public async Task<IActionResult> Update(long id, CategoryUpdateDto entity)
        {
            await _categoryService.UpdateAsync(id, entity);
            return NoContent();
        }


        [HttpDelete("{id:long:min(1)}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();

        }
    }
}
