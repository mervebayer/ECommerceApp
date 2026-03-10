using ECommerceApp.Application.DTOs.QueryParams;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.Users
{
    public class UserQueryParamsValidator : AbstractValidator<UserQueryParams>
    {
        public UserQueryParamsValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.PageSize).InclusiveBetween(1, 50).WithMessage("PageSize must be between 1 and 50.");
        }
    }
}
