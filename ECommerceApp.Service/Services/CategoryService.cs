using AutoMapper;
using ECommerceApp.Core.DTOs.Categories;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Exceptions;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Core.Interfaces.Repositories;
using ECommerceApp.Core.Interfaces.Services;
using ECommerceApp.Service.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CategoryCreateDto> _createValidator;
        private readonly IValidator<CategoryUpdateDto> _updateValidator;
        private readonly ILogger<ProductService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, IGenericRepository<Category> categoryRepository, IMapper mapper, IValidator<CategoryCreateDto> createValidator, IValidator<CategoryUpdateDto> updateValidator, IGenericRepository<Product> productRepository, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var data = await _categoryRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<CategoryDto>>(data);
        }

        public async Task<CategoryDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var data = await _categoryRepository.GetByIdAsync(id, cancellationToken) ??
                throw new NotFoundException($"Category with Id {id} was not found.");
            return _mapper.Map<CategoryDto>(data);
        }

        public async Task<CategoryDto> AddAsync(CategoryCreateDto entity, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(entity, cancellationToken);
            validationResult.ThrowIfInvalid();

            var data = _mapper.Map<Category>(entity);
            await _categoryRepository.AddAsync(data, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return _mapper.Map<CategoryDto>(data);
        }

        public async Task UpdateAsync(long id, CategoryUpdateDto entity, CancellationToken cancellationToken)
        {
            entity.Id = id;
            var data = await _categoryRepository.GetByIdAsync(id, cancellationToken) ??
                throw new NotFoundException($"Category with Id {id} was not found.");
    

            var validationResult = await _updateValidator.ValidateAsync(entity, cancellationToken);
            validationResult.ThrowIfInvalid();

            var category = _mapper.Map(entity, data);
            _categoryRepository.Update(category);
            await _unitOfWork.CommitAsync(cancellationToken);

        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken)
        {
            var data = await _categoryRepository.GetByIdAsync(id, cancellationToken) ??
                throw new NotFoundException($"Category with Id {id} was not found.");

            var hasProducts = await _productRepository.AnyAsync(x => x.CategoryId == id, cancellationToken);

            if (hasProducts)
            {
                _logger.LogWarning("Category {CategoryId} delete attempt failed. It has active products.", id); 
                throw new BadRequestException("This category cannot be deleted because it contains active products.");
            }

            _categoryRepository.Delete(data);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Category {CategoryId} deleted successfully.", id);
        }
        
    }
}
