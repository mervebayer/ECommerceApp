using ECommerceApp.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IProductDescriptionGenerator
    {
        Task<string> GenerateAsync(ProductDescriptionGenerationInput input, CancellationToken cancellationToken = default);
    }
}
