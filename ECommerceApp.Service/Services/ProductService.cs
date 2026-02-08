using AutoMapper;
using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Core.Interfaces.Repositories;
using ECommerceApp.Core.Interfaces.Services;

namespace ECommerceApp.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _product;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IGenericRepository<Product> product, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _product = product;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDto> AddAsync(ProductCreateDto entity)
        {
           var data = _mapper.Map<Product>(entity);    
           await _product.AddAsync(data);
           await _unitOfWork.CommitAsync();
           return _mapper.Map<ProductDto>(data);
        }

        public async Task Delete(long id)
        {
            var data = await _product.GetByIdAsync(id);
            if (data == null) 
                throw new KeyNotFoundException("Data does not exist.");
            _product.Delete(data);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var data = await _product.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }

        public async Task<ProductDto> GetByIdAsync(long id)
        {
            var data = await _product.GetByIdAsync(id);
            if (data == null)
                throw new KeyNotFoundException("Data does not exist.");
            return _mapper.Map<ProductDto>(data);   
        }

        public async Task<ProductDto> Update(ProductUpdateDto entity)
        {
            var data = await _product.GetByIdAsync(entity.Id);
            if (data == null)
                throw new KeyNotFoundException("Data does not exist.");
            var product = _mapper.Map(entity, data);
            _product.Update(product);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProductDto>(product); 
        }
    }
}
