using ECommerceApp.Core.DTOs.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Validations.Images
{
    public class ImageUploadDtoValidator : AbstractValidator<ImageUploadDto>
    {
        public ImageUploadDtoValidator()
        {
            RuleFor(x => x.Length).GreaterThan(0).WithMessage("Empty file.");
            RuleFor(x => x.ContentType).Must(x => x == "image/jpeg" || x == "image/png" || x == "image/webp").WithMessage("Only jpg/png/webp allowed.");
        }
    }
}
