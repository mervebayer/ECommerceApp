using ECommerceApp.Application.DTOs.UserProfiles;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.UserProfiles
{
    public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
    {
        public UpdateUserProfileDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.")
                                                .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
                                                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.")
                                     .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
                                     .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.")
                                      .MinimumLength(4).WithMessage("UserName must be at least 4 characters.")
                                      .MaximumLength(20).WithMessage("UserName cannot exceed 20 characters.")
                                      .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("UserName can only contain letters, numbers, dot and underscore.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                 .EmailAddress().WithMessage("Email is not a valid email address.");
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Phone number must contain only digits and may start with '+'.");

        }
    }
}
