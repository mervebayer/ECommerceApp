using ECommerceApp.Application.DTOs.Orders;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.Orders
{
    public class CreateOrderRequestDtoValidator : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderRequestDtoValidator()
        {
            RuleFor(x => x.UserAddressId)
                .GreaterThan(0)
                .WithMessage("A valid address must be selected.");
        }
    }
}
