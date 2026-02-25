using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerceApp.Application.DTOs.Images;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Infrastructure.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.ExternalServices
{
    public class CloudinaryImageStorage : IImageStorage
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorage(IOptions<CloudinaryOptions> options)
        {
            var o = options.Value;
            _cloudinary = new Cloudinary(new Account(o.CloudName, o.ApiKey, o.ApiSecret));
        }

        public async Task<FileSaveResult> UploadProductImageAsync(long productId, ImageUploadDto image, CancellationToken ct)
        {
            var publicId = $"products/{productId}/{Guid.NewGuid():N}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, image.Content),
                PublicId = publicId,
                Overwrite = false,
                Transformation = new Transformation()
                    .Width(1200).Height(1200).Crop("limit")
                    .Quality("auto").FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams, ct);

            if (result.Error is not null)
                throw new Exception(result.Error.Message);

            return new FileSaveResult(result.PublicId, result.SecureUrl.ToString());
        }

        public async Task DeleteAsync(string publicId, CancellationToken ct)
        {
            var delParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
            await _cloudinary.DestroyAsync(delParams);
        }
    }
}
