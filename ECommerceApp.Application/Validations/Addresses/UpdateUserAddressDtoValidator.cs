using ECommerceApp.Application.DTOs.Addresses;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.Addresses
{
    public class UpdateUserAddressDtoValidator : AbstractValidator<UpdateUserAddressDto>
    {
        public UpdateUserAddressDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("{PropertyName} is required.").MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");
            RuleFor(x => x.ContactName).NotEmpty().WithMessage("{PropertyName} is required.").MaximumLength(150).WithMessage("{PropertyName} cannot exceed 150 characters.");
            RuleFor(x => x.Country).NotEmpty().WithMessage("{PropertyName} is required.").MaximumLength(60).WithMessage("{PropertyName} cannot exceed 60 characters.");
            RuleFor(x => x.City).NotEmpty().WithMessage("{PropertyName} is required.").MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.");
            RuleFor(x => x.District).NotEmpty().WithMessage("{PropertyName} is required.").MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.");
            RuleFor(x => x.PostalCode).NotEmpty().WithMessage("{PropertyName} is required.").MaximumLength(20).WithMessage("{PropertyName} cannot exceed 20 characters.");
            RuleFor(x => x.AddressLine).NotEmpty().WithMessage("{PropertyName} is required.").MaximumLength(500).WithMessage("{PropertyName} cannot exceed 500 characters.");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required.")
                                        .Matches(@"^\+?\d{10,15}$")
                                        .WithMessage("Phone number must contain only digits and may start with '+'.");
        }
    }
}
