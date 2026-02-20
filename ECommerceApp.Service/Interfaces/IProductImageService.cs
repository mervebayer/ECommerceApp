using ECommerceApp.Core.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Interfaces
{
    public interface IProductImageService
    {
        Task<ProductImageDto> AddImageAsync(long productId, ImageUploadDto image, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(long productId, long imageId, CancellationToken cancellationToken = default);
        Task SetMainImageAsync(long productId, long imageId, CancellationToken cancellationToken = default);
    }
}
