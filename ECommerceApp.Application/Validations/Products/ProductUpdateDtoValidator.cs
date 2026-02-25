using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.Products
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public ProductUpdateDtoValidator(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;

            //RuleFor(x => x.Id).GreaterThan(0).WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be 0 or more.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("{PropertyName} is required.");
            RuleFor(x => x.CategoryId)
                .MustAsync(async (id, ct) =>
                    await _categoryRepository.AnyAsync(x => x.Id == id, ct))
                .WithMessage("Category not found.");
           
        }
    }
}
