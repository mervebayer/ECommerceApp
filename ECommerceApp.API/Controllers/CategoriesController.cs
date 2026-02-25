using ECommerceApp.Application.DTOs.Categories;
using ECommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin,StoreManager,Customer")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var data = await _categoryService.GetAllAsync(cancellationToken);
            return Ok(data);
        }
        [HttpGet("{id:long:min(1)}")]
        [Authorize(Roles = "Admin,StoreManager,Customer")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            var data = await _categoryService.GetByIdAsync(id, cancellationToken);
            return Ok(data);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto entity, CancellationToken cancellationToken) { 
            var data = await _categoryService.AddAsync(entity, cancellationToken);
            return StatusCode(201, data);
        }

        [HttpPut("{id:long:min(1)}")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Update(long id, [FromBody] CategoryUpdateDto entity, CancellationToken cancellationToken)
        {
            await _categoryService.UpdateAsync(id, entity, cancellationToken);
            return NoContent();
        }


        [HttpDelete("{id:long:min(1)}")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await _categoryService.DeleteAsync(id, cancellationToken);
            return NoContent();

        }
    }
}
