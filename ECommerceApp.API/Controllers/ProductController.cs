using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class ProductController : ControllerBase
    {
        private readonly IGenericRepository<Product> _product;
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IGenericRepository<Product> product, IUnitOfWork unitOfWork)
        {
            _product = product;
            _unitOfWork = unitOfWork;   
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _product.GetAllAsync();
            return Ok(data);
        }
        
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _product.GetByIdAsync(id);
            if(data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _product.AddAsync(product);
            await _unitOfWork.CommitAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Product product)
        {
            var data = await _product.GetByIdAsync(product.Id);
            if (data == null) return NotFound();
            _product.Update(product);
            await _unitOfWork.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id) { 
            var data = await _product.GetByIdAsync(id);
            if(data == null) return NotFound();
            _product.Delete(data);
            await _unitOfWork.CommitAsync();
            return NoContent();
        }
             
    }
}
