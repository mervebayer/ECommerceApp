using ECommerceApp.Core.DTOs.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Validations.Users
{
    public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.")
                                     .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
                                     .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.")
                                     .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
                                     .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.")
                                  .MinimumLength(2).WithMessage("City must be at least 2 characters.")
                                  .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.")
                                      .MinimumLength(5).WithMessage("Address must be at least 5 characters.")
                                      .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.")
                                      .MinimumLength(4).WithMessage("UserName must be at least 4 characters.")
                                      .MaximumLength(20).WithMessage("UserName cannot exceed 20 characters.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                                     .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                                     .MaximumLength(25).WithMessage("Password cannot exceed 25 characters.")
                                     .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                                     .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                                     .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                                     .Matches(@"[#$@^&]").WithMessage("Password must contain at least one special character.");

        }
    }
}