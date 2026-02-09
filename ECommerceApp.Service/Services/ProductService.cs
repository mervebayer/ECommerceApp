using AutoMapper;
using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Enums;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Core.Interfaces.Repositories;
using ECommerceApp.Core.Interfaces.Services;

namespace ECommerceApp.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _product;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository product, IMapper mapper, IUnitOfWork unitOfWork)
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
            var product = await _product.GetByIdWithCategoryAsync(data.Id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task Delete(long id)
        {
            var data = await _product.GetByIdAsync(id);
            if (data == null)
                throw new KeyNotFoundException("Data does not exist.");
            _product.Delete(data);
            await _unitOfWork.CommitAsync();
        }
        public async Task<IEnumerable<ProductDto>> GetAllAsync(int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest)
        {
            var data = await _product.GetAllWithCategoriesAsync(pageSize, pageNumber, sortType);
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }
        public async Task<IEnumerable<ProductDto>> GetAllWithoutCategoryAsync()
        {
            var data = await _product.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }

        public async Task<ProductDto> GetByIdAsync(long id)
        {
            var data = await _product.GetByIdWithCategoryAsync(id);
            if (data == null)
                throw new KeyNotFoundException("Data does not exist.");
            return _mapper.Map<ProductDto>(data);
        }
        public async Task<ProductDto> GetByIdWithoutCategoryAsync(long id)
        {
            var data = await _product.GetByIdAsync(id);
            if (data == null)
                throw new KeyNotFoundException("Data does not exist.");
            return _mapper.Map<ProductDto>(data);
        }


        // TODO: update category name

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

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest) {
            var data = await _product.GetProductsByCategoryIdAsync(categoryId, pageSize, pageNumber, sortType);    
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }
    }
}
