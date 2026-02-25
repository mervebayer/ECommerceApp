using AutoMapper;
using ECommerceApp.Application.DTOs.Images;
using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.Extensions;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Exceptions;
using ECommerceApp.Domain.Interfaces;
using ECommerceApp.Domain.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IImageStorage _imageStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ImageUploadDto> _uploadValidator;
        private readonly ILogger<ProductService> _logger;
        public ProductImageService(IProductRepository productRepository, IProductImageRepository productImageRepository, IImageStorage imageStorage, IUnitOfWork unitOfWork, IMapper mapper, IValidator<ImageUploadDto> uploadValidator, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _imageStorage = imageStorage;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadValidator = uploadValidator;
            _logger = logger;
        }

        public async Task<ProductImageDto> AddImageAsync(long productId, ImageUploadDto image, CancellationToken cancellationToken)
        {
            var validationResult = await _uploadValidator.ValidateAsync(image, cancellationToken);
            validationResult.ThrowIfInvalid();

            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null) throw new NotFoundException("Product not found.");
  

            var count = await _productImageRepository.CountByProductIdAsync(productId, cancellationToken);
            if (count >= 5) throw new BadRequestException("Max 5 images per product.");

            var saved = await _imageStorage.UploadProductImageAsync(productId, image, cancellationToken);


            var entity = new ProductImage
            {
                ProductId = productId,
                Url = saved.Url,
                PublicId = saved.PublicId,
                IsMain = count == 0 
            };

            try
            {
                await _productImageRepository.AddAsync(entity, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                return _mapper.Map<ProductImageDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist image metadata. ProductId={ProductId}, PublicId={PublicId}", productId, saved.PublicId);
                try
                {
                    await _imageStorage.DeleteAsync(saved.PublicId, cancellationToken);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to cleanup Cloudinary file after DB failure. ProductId={ProductId}, PublicId={PublicId}", productId, saved.PublicId);
                }
                throw;
            }
        }
        public async Task DeleteImageAsync(long productId, long imageId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null) throw new NotFoundException("Product not found.");

            var image = await _productImageRepository.GetByIdAsync(imageId, cancellationToken);
            if (image is null) throw new NotFoundException("Image not found.");

            if (image.ProductId != productId)
                throw new BadRequestException("Image does not belong to this product.");

            var wasMain = image.IsMain;
            var publicId = image.PublicId;

            _productImageRepository.Delete(image);
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Product image deleted from DB. ProductId={ProductId}, ImageId={ImageId}, WasMain={WasMain}", productId, imageId, wasMain);

            try
            {
                await _imageStorage.DeleteAsync(publicId, cancellationToken);

                _logger.LogInformation("Product image deleted from storage. ProductId={ProductId}, PublicId={PublicId}", productId, publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to delete image from storage. ProductId={ProductId}, PublicId={PublicId}", productId, publicId);
            }


            if (wasMain)
            {
                var remaining = await _productImageRepository.GetByProductIdAsync(productId, cancellationToken);
                var newMain = remaining.OrderBy(x => x.Id).FirstOrDefault();
                if (newMain is not null && !newMain.IsMain)
                {
                    newMain.IsMain = true;
                    _productImageRepository.Update(newMain);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    _logger.LogInformation("Main image reassigned after deletion. ProductId={ProductId}, NewMainImageId={ImageId}", productId, newMain.Id);
                }
            }
        }

        public async Task SetMainImageAsync(long productId, long imageId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null) throw new NotFoundException("Product not found.");

            var images = await _productImageRepository.GetByProductIdAsync(productId, cancellationToken);
            if (images.Count == 0) throw new BadRequestException("Product has no images.");

            var target = images.FirstOrDefault(x => x.Id == imageId);
            if (target is null) throw new NotFoundException("Image not found.");

            foreach (var img in images)
                img.IsMain = img.Id == target.Id;

            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Main image set. ProductId={ProductId}, ImageId={ImageId}", productId, imageId);
        }
    }
}
