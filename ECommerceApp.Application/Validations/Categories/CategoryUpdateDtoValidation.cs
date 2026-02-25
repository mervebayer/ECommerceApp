using ECommerceApp.Application.DTOs.Categories;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Interfaces.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.Categories
{
    public class CategoryUpdateDtoValidation : AbstractValidator<CategoryUpdateDto>
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public CategoryUpdateDtoValidation(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
            RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} is required.")
                                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.")
                                //.MustAsync(BeUniqueName)
                                .WithMessage("A category with this name already exists.");
        }

        //private async Task<bool> BeUniqueName(CategoryUpdateDto dto, string name, CancellationToken cancellationToken)
        //{
        //    bool exists = await _categoryRepository
        //        .Where(x => x.Name == name && x.Id != dto.Id)
        //        .AnyAsync(cancellationToken);
        //    return !exists;
        //}
    }
}
