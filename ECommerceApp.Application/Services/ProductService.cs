using AutoMapper;
using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Extensions;
using ECommerceApp.Application.Interfaces;

using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using ECommerceApp.Domain.Exceptions;
using ECommerceApp.Domain.Interfaces;
using ECommerceApp.Domain.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ECommerceApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;
        private readonly IValidator<ProductQueryParams> _paramValidator;
        private readonly ILogger<ProductService> _logger;
        private readonly IProductDescriptionGenerator _productDescriptionGenerator;

        public ProductService(IProductRepository productRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<ProductCreateDto> createValidator, IValidator<ProductUpdateDto> updateValidator, ILogger<ProductService> logger, IValidator<ProductQueryParams> paramValidator, IProductReadRepository productReadRepository, IProductDescriptionGenerator productDescriptionGenerator)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
            _paramValidator = paramValidator;
            _productReadRepository = productReadRepository;
            _productDescriptionGenerator = productDescriptionGenerator;
        }

        public async Task<PagedResult<ProductListItemDto>> GetAllAsync(ProductQueryParams p, CancellationToken cancellationToken)
        {
            var validationResult = await _paramValidator.ValidateAsync(p, cancellationToken);
            validationResult.ThrowIfInvalid();

            return await _productReadRepository.GetProductsAsync(p, cancellationToken);
        }    

        public async Task<ProductDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var data = await _productRepository.GetProductByIdAsync(id, cancellationToken) ??
                throw new NotFoundException($"Product with Id {id} was not found.");
            return _mapper.Map<ProductDto>(data);
        }

        public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId, pageSize, pageNumber, sortType, cancellationToken);

            return products.Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                MainImageUrl = p.Images.FirstOrDefault(i => i.IsMain)?.Url,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate
            }).ToList();
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

        public async Task<GenerateProductDescriptionResponseDto> GenerateDescriptionAsync(long id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Product with Id {id} was not found.");

            var input = new ProductDescriptionGenerationInput
            {
                Name = product.Name,
                CurrentDescription = product.Description,
                CategoryName = product.Category?.Name,
                Price = product.Price,
                ImageUrl = product.Images
                    .OrderByDescending(x => x.IsMain)
                    .Select(x => x.Url)
                    .FirstOrDefault()
            };

            var generatedDescription = await _productDescriptionGenerator.GenerateAsync(input, cancellationToken);

            return new GenerateProductDescriptionResponseDto
            {
                Description = generatedDescription
            };
        }
        public async Task<ProductDto> ApplyGeneratedDescriptionAsync(long id, ApplyGeneratedDescriptionRequestDto request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new BadRequestException("Request cannot be null.");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new BadRequestException("Description is required.");

            if (request.Description.Length > 4000)
                throw new BadRequestException("Description cannot exceed 4000 characters.");

            var product = await _productRepository.GetProductByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Product with Id {id} was not found.");

            product.Description = request.Description.Trim();

            _productRepository.Update(product);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Generated product description applied. ProductId={ProductId}", id);

            var updatedProduct = await _productRepository.GetProductByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Product with Id {id} could not be loaded after update.");

            return _mapper.Map<ProductDto>(updatedProduct);
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
