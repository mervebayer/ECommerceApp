using ECommerceApp.Application.DTOs.UserProfiles;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Validations.UserProfiles
{
    public class ChangePasswordDtoValidator: AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters.")
                .MaximumLength(25).WithMessage("New password cannot exceed 25 characters.")
                .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("New password must contain at least one number.")
                .Matches(@"[#$@^&.]").WithMessage("New password must contain at least one special character.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword).WithMessage("New password and confirm password must match.");
        }

    }

}


