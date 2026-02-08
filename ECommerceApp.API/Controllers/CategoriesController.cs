using ECommerceApp.Core.DTOs.Categories;
using ECommerceApp.Core.Interfaces.Services;
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

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDto entity) { 
            var data = await _categoryService.AddAsync(entity);
            return StatusCode(201, data);
        }
    }
}
