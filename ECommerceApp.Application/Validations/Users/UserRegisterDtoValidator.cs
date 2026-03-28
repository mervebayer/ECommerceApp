using ECommerceApp.Application.DTOs.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.Users
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
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.")
                                      .MinimumLength(4).WithMessage("UserName must be at least 4 characters.")
                                      .MaximumLength(20).WithMessage("UserName cannot exceed 20 characters.")
                                      .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("UserName can only contain letters, numbers, dot and underscore.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                 .EmailAddress().WithMessage("Email is not a valid email address.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                                     .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                                     .MaximumLength(25).WithMessage("Password cannot exceed 25 characters.")
                                     .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                                     .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                                     .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                                     .Matches(@"[#$@^&.]").WithMessage("Password must contain at least one special character.");

        }
    }
}