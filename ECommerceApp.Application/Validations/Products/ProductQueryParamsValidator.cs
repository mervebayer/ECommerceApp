using ECommerceApp.Application.DTOs.QueryParams;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.Products
{
    public class ProductQueryParamsValidator : AbstractValidator<ProductQueryParams>
    {
        public ProductQueryParamsValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
            RuleFor(x => x).Must(x => x.MinPrice <= x.MaxPrice ).When(x => x.MinPrice.HasValue &&  x.MaxPrice.HasValue);
        }
    }
}
