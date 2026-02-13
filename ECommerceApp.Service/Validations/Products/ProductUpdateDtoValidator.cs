using ECommerceApp.Core.DTOs.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Validations.Products
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            //RuleFor(x => x.Id).GreaterThan(0).WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be 0 or more.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("{PropertyName} is required.");

        }
    }
}
