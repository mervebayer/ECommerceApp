using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Extensions
{
    public static class ValidationResultExtensions
    {
        public static void ThrowIfInvalid(this ValidationResult result)
        {
            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );
                throw new Core.Exceptions.ValidationException(errors);
            }
        }
    }
}
