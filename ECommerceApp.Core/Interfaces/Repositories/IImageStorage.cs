using ECommerceApp.Core.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Interfaces.Repositories
{
    public interface IImageStorage
    {
        Task<FileSaveResult> UploadProductImageAsync(long productId, ImageUploadDto image, CancellationToken ct);
        Task DeleteAsync(string publicId, CancellationToken ct);
    }
}
