using ECommerceApp.Application.DTOs.Images;
using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;

        public ProductsController(IProductService productService, IProductImageService productImageService)
        {
            _productService = productService;
            _productImageService = productImageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default)
        {
            var data = await _productService.GetAllAsync(pageNumber, pageSize, sortType, cancellationToken);
            return Ok(data);
        }
        
        [HttpGet("{id:long:min(1)}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
        {
            var data = await _productService.GetByIdAsync(id, cancellationToken);
            return Ok(data);
        }


        [HttpGet("category/{categoryId:long:min(1)}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategoryId([FromRoute] long categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default) { 
            var data = await _productService.GetProductsByCategoryIdAsync(categoryId, pageNumber, pageSize, sortType, cancellationToken);
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto product, CancellationToken cancellationToken = default)
        {
            var data = await _productService.AddAsync(product, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
        }

        [HttpPut("{id:long:min(1)}")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Update(long id, [FromBody] ProductUpdateDto product, CancellationToken cancellationToken = default)
        {
                await _productService.UpdateAsync(id, product, cancellationToken);
                return NoContent();       
        }

        [HttpDelete("{id:long:min(1)}")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
        {
            await _productService.DeleteAsync(id, cancellationToken);
            return NoContent();      
          
        }

        [HttpPost("{id:long}/images")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> UploadImage(long id, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file is null || file.Length == 0) return BadRequest("File is required.");

            await using var stream = file.OpenReadStream();
            var dto = new ImageUploadDto
            {
                Content = stream,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length
            };
            var result = await _productImageService.AddImageAsync(id, dto, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{productId:long}/images/{imageId:long}")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> DeleteImage(long productId, long imageId, CancellationToken cancellationToken = default)
        {
            await _productImageService.DeleteImageAsync(productId, imageId, cancellationToken);
            return NoContent();
        }

        [HttpPut("{productId:long}/images/{imageId:long}/main")]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> SetMainImage(long productId, long imageId, CancellationToken cancellationToken = default)
        {
            await _productImageService.SetMainImageAsync(productId, imageId, cancellationToken);
            return NoContent();
        }

    }
}
