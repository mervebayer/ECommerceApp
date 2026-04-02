using AutoMapper;
using ECommerceApp.Application.DTOs.UserFavorites;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Exceptions;
using ECommerceApp.Domain.Interfaces;
using ECommerceApp.Domain.Interfaces.Repositories;

namespace ECommerceApp.Application.Services
{
    public class UserFavoriteService : IUserFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserFavoriteService(
            IFavoriteRepository favoriteRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _favoriteRepository = favoriteRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task AddToFavoritesAsync(string userId, long productId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
                throw new NotFoundException("Product not found.");

            var exists = await _favoriteRepository.ExistsAsync(userId, productId, cancellationToken);
            if (exists)
                return;

            var favorite = new UserFavorite
            {
                UserId = userId,
                ProductId = productId
            };

            await _favoriteRepository.AddAsync(favorite, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FavoriteItemDto>> GetMyFavoritesAsync(string userId, CancellationToken cancellationToken = default)
        {
            var favorites = await _favoriteRepository.GetByUserIdAsync(userId, cancellationToken);

            return _mapper.Map<List<FavoriteItemDto>>(favorites);
        }

        public async Task<bool> IsFavoriteAsync(string userId, long productId, CancellationToken cancellationToken = default)
        {
            return await _favoriteRepository.ExistsAsync(userId, productId, cancellationToken);
        }

        public async Task RemoveFromFavoritesAsync(string userId, long productId, CancellationToken cancellationToken = default)
        {
            var favorite = await _favoriteRepository.GetByUserIdAndProductIdAsync(userId, productId, cancellationToken);
            if (favorite is null)
                return;

            _favoriteRepository.Remove(favorite);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
