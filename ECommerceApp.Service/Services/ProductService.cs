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
        private readonly IProductRepository _product;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;

        public ProductService(IProductRepository product, IMapper mapper, IUnitOfWork unitOfWork, IValidator<ProductCreateDto> createValidator, IValidator<ProductUpdateDto> updateValidator)
        {
            _product = product;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest)
        {
            var data = await _product.GetAllWithCategoriesAsync(pageSize, pageNumber, sortType);
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }
        public async Task<ProductDto> GetByIdAsync(long id)
        {
            var data = await _product.GetByIdWithCategoryAsync(id);
            if (data == null)
                throw new NotFoundException($"Product with Id {id} was not found.");
            return _mapper.Map<ProductDto>(data);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(long categoryId, int pageNumber = 1, int pageSize = 20, ProductSortType sortType = ProductSortType.Newest)
        {
            var data = await _product.GetProductsByCategoryIdAsync(categoryId, pageSize, pageNumber, sortType);
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }

        public async Task<ProductDto> AddAsync(ProductCreateDto entity)
        {
            var validationResult = await _createValidator.ValidateAsync(entity);
            validationResult.ThrowIfInvalid();
        
            var data = _mapper.Map<Product>(entity);
            await _product.AddAsync(data);
            await _unitOfWork.CommitAsync();
            var product = await _product.GetByIdWithCategoryAsync(data.Id);
            return _mapper.Map<ProductDto>(product);
        }

        // TODO: update category name

        public async Task<ProductDto> Update(ProductUpdateDto entity)
        {
            var validationResult = await _updateValidator.ValidateAsync(entity);
            validationResult.ThrowIfInvalid();

            var data = await _product.GetByIdAsync(entity.Id);
            if (data == null) 
                throw new NotFoundException($"Product with Id {entity.Id} was not found.");
                           
            var product = _mapper.Map(entity, data);
            _product.Update(product);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task Delete(long id)
        {
            if (id <= 0)
            {
                var errors = new Dictionary<string, string[]>
                    {
                        { "Id", new[] { "Id must be greater than 0." } }
                    };
                throw new Core.Exceptions.ValidationException(errors);
            }
            var data = await _product.GetByIdAsync(id);
            if (data == null)
                throw new NotFoundException($"Product with Id {id} was not found.");
            _product.Delete(data);
            await _unitOfWork.CommitAsync();
        }




        #region WithoutCategory
        public async Task<IEnumerable<ProductDto>> GetAllWithoutCategoryAsync()
        {
            var data = await _product.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(data);
        }

        public async Task<ProductDto> GetByIdWithoutCategoryAsync(long id)
        {
            var data = await _product.GetByIdAsync(id);
            if (data == null)
                throw new KeyNotFoundException("Data does not exist.");
            return _mapper.Map<ProductDto>(data);
        }


        #endregion


    }
}
