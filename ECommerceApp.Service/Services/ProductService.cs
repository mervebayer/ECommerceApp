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
using Microsoft.Extensions.Logging;

namespace ECommerceApp.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<ProductCreateDto> createValidator, IValidator<ProductUpdateDto> updateValidator, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductListDto>> GetAllAsync(int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetProductList(pageSize, pageNumber, sortType, cancellationToken);
        }
        public async Task<ProductDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var data = await _productRepository.GetProductByIdAsync(id, cancellationToken) ??
                throw new NotFoundException($"Product with Id {id} was not found.");
            return _mapper.Map<ProductDto>(data);
        }

        public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetProductsByCategoryIdAsync(categoryId, pageSize, pageNumber, sortType, cancellationToken);
        }

        public async Task<ProductDto> AddAsync(ProductCreateDto entity, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(entity, cancellationToken);
            validationResult.ThrowIfInvalid();
        
            var data = _mapper.Map<Product>(entity);
            await _productRepository.AddAsync(data, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Product created. ProductId={ProductId}, CategoryId={CategoryId}, Price={Price}, Stock={Stock}", data.Id, data.CategoryId, data.Price, data.Stock);

            var product = await _productRepository.GetProductByIdAsync(data.Id, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Product created but could not be loaded after creation. ProductId={ProductId}", data.Id);

                throw new NotFoundException($"Product with Id {data.Id} could not be loaded after creation.");
            }
            return _mapper.Map<ProductDto>(product);
        }


        // TODO: update category name
        public async Task UpdateAsync(long id, ProductUpdateDto entity, CancellationToken cancellationToken)
        {
            var validationResult = await _updateValidator.ValidateAsync(entity, cancellationToken);
            validationResult.ThrowIfInvalid();

            var data = await _productRepository.GetByIdAsync(id, cancellationToken) ??
                throw new NotFoundException($"Product with Id {id} was not found.");

            var product = _mapper.Map(entity, data);
            _productRepository.Update(product);
            await _unitOfWork.CommitAsync(cancellationToken);
        }


        public async Task DeleteAsync(long id, CancellationToken cancellationToken)
        {
            var data = await _productRepository.GetByIdAsync(id, cancellationToken) ??
                throw new NotFoundException($"Product with Id {id} was not found.");
            _productRepository.Delete(data);
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Product deleted (soft delete). ProductId={ProductId}", id);
        }


        #region WithoutCategory
        public async Task<IEnumerable<ProductDto>> GetAllWithoutCategoryAsync(CancellationToken cancellationToken)
        {
            var data = await _productRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }

        public async Task<ProductDto> GetByIdWithoutCategoryAsync(long id, CancellationToken cancellationToken)
        {
            var data = await _productRepository.GetByIdAsync(id, cancellationToken) ??
                throw new KeyNotFoundException("Data does not exist.");
            return _mapper.Map<ProductDto>(data);
        }


        #endregion


    }
}
