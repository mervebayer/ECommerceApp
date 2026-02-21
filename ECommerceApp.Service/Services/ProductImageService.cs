using AutoMapper;
using ECommerceApp.Core.DTOs.Products;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Exceptions;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Core.Interfaces.Repositories;
using ECommerceApp.Core.Interfaces.Services;
using ECommerceApp.Service.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IImageStorage _imageStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ImageUploadDto> _uploadValidator;

        public ProductImageService(IProductRepository productRepository, IProductImageRepository productImageRepository, IImageStorage imageStorage, IUnitOfWork unitOfWork, IMapper mapper, IValidator<ImageUploadDto> uploadValidator)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _imageStorage = imageStorage;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadValidator = uploadValidator;
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

            await _productImageRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return _mapper.Map<ProductImageDto>(entity);
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


            await _imageStorage.DeleteAsync(publicId, cancellationToken);

            if (wasMain)
            {
                var remaining = await _productImageRepository.GetByProductIdAsync(productId, cancellationToken);
                var newMain = remaining.FirstOrDefault();
                if (newMain is not null && !newMain.IsMain)
                {
                    newMain.IsMain = true;
                    _productImageRepository.Update(newMain);
                    await _unitOfWork.CommitAsync(cancellationToken);
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
        }
    }
}
