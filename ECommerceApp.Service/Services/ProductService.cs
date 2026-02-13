using AutoMapper;
using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Enums;
using ECommerceApp.Core.Exceptions;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Core.Interfaces.Repositories;
using ECommerceApp.Core.Interfaces.Services;
using ECommerceApp.Service.Extensions;
using FluentValidation;

namespace ECommerceApp.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;

        public ProductService(IProductRepository productRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<ProductCreateDto> createValidator, IValidator<ProductUpdateDto> updateValidator)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest)
        {
            var data = await _productRepository.GetAllWithCategoriesAsync(pageSize, pageNumber, sortType);
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }
        public async Task<ProductDto> GetByIdAsync(long id)
        {
            var data = await _productRepository.GetByIdWithCategoryAsync(id) ??
                throw new NotFoundException($"Product with Id {id} was not found.");
            return _mapper.Map<ProductDto>(data);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest)
        {
            var data = await _productRepository.GetProductsByCategoryIdAsync(categoryId, pageSize, pageNumber, sortType);
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }

        public async Task<ProductDto> AddAsync(ProductCreateDto entity)
        {
            var validationResult = await _createValidator.ValidateAsync(entity);
            validationResult.ThrowIfInvalid();
        
            var data = _mapper.Map<Product>(entity);
            await _productRepository.AddAsync(data);
            await _unitOfWork.CommitAsync();
            var product = await _productRepository.GetByIdWithCategoryAsync(data.Id) ??
                throw new NotFoundException($"Product with Id {data.Id} could not be loaded after creation.");       
            return _mapper.Map<ProductDto>(product);
        }


        // TODO: update category name
        public async Task UpdateAsync(long id, ProductUpdateDto entity)
        {
            var validationResult = await _updateValidator.ValidateAsync(entity);
            validationResult.ThrowIfInvalid();

            var data = await _productRepository.GetByIdAsync(id) ??
                throw new NotFoundException($"Product with Id {id} was not found.");
                           
            var product = _mapper.Map(entity, data);
            _productRepository.Update(product);
            await _unitOfWork.CommitAsync();         
        }


        public async Task DeleteAsync(long id)
        {
            var data = await _productRepository.GetByIdAsync(id) ??
                throw new NotFoundException($"Product with Id {id} was not found.");
            _productRepository.Delete(data);
            await _unitOfWork.CommitAsync();
        }

        //public async Task<ProductDto> Update(long id, ProductUpdateDto entity)
        //{
        //    var validationResult = await _updateValidator.ValidateAsync(entity);
        //    validationResult.ThrowIfInvalid();

        //    var data = await _product.GetByIdAsync(id);
        //    if (data == null) 
        //        throw new NotFoundException($"Product with Id {id} was not found.");

        //    var product = _mapper.Map(entity, data);
        //    _product.Update(product);
        //    await _unitOfWork.CommitAsync();
        //    return _mapper.Map<ProductDto>(product);
        //}


        #region WithoutCategory
        public async Task<IEnumerable<ProductDto>> GetAllWithoutCategoryAsync()
        {
            var data = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }

        public async Task<ProductDto> GetByIdWithoutCategoryAsync(long id)
        {
            var data = await _productRepository.GetByIdAsync(id) ??
                throw new KeyNotFoundException("Data does not exist.");
            return _mapper.Map<ProductDto>(data);
        }


        #endregion


    }
}
