using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Enums;
using ECommerceApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] ProductSortType sortType = ProductSortType.Newest)
        {
            var data = await _productService.GetAllAsync(pageNumber, pageSize, sortType);
            return Ok(data);
        }
        
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _productService.GetByIdAsync(id);
            return Ok(data);
        }
   

        [HttpGet("category/{categoryId:long}")]
        public async Task<IActionResult> GetByCategoryId([FromRoute] long categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] ProductSortType sortType = ProductSortType.Newest) { 
            var data = await _productService.GetProductsByCategoryIdAsync(categoryId, pageNumber, pageSize, sortType);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto product)
        {
            var data = await _productService.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, ProductUpdateDto product)
        {
            var data =  await _productService.Update(product);
                //return Ok(data);
                return NoContent();       
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _productService.Delete(id);
            return NoContent();      
          
        }
             
    }
}
