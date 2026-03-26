using AutoMapper;
using ECommerceApp.Application.DTOs.Addresses;
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
    public class UserAddressService : IUserAddressService
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateUserAddressDto> _createValidator;
        private readonly IValidator<UpdateUserAddressDto> _updateValidator;
        private readonly ILogger<UserAddressService> _logger;

        public UserAddressService(IUserAddressRepository userAddressRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateUserAddressDto> createValidator, IValidator<UpdateUserAddressDto> updateValidator, ILogger<UserAddressService> logger)
        {
            _userAddressRepository = userAddressRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<IReadOnlyList<UserAddressDto>> GetMyAddressesAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var addresses = await _userAddressRepository.GetByUserIdAsync(userId, cancellationToken);
            return _mapper.Map<List<UserAddressDto>>(addresses);
        }
        public async Task<UserAddressDto> GetMyAddressByIdAsync(string userId, long addressId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var address = await _userAddressRepository.GetByIdAndUserIdAsync(addressId, userId, cancellationToken);

            if (address is null)
            {
                _logger.LogWarning("User address not found. UserId={UserId}, AddressId={AddressId}", userId, addressId);
                throw new NotFoundException("Address not found.");
            }
            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task<UserAddressDto> CreateAsync(string userId, CreateUserAddressDto request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            validationResult.ThrowIfInvalid();

            var existingAddresses = await _userAddressRepository.GetByUserIdAsync(userId, cancellationToken);

            var address = _mapper.Map<UserAddress>(request);
            address.UserId = userId;

            var hasAnyAddress = existingAddresses.Any();
            var currentDefaultAddress = existingAddresses.SingleOrDefault(x => x.IsDefault);

            if (!hasAnyAddress)
            {
                address.IsDefault = true;
            }
            else if (request.IsDefault)
            {
                if (currentDefaultAddress is not null)
                {
                    currentDefaultAddress.IsDefault = false;
                    _userAddressRepository.Update(currentDefaultAddress);
                }

                address.IsDefault = true;
            }

            await _userAddressRepository.AddAsync(address, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task SetDefaultAsync(string userId, long addressId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var address = await _userAddressRepository.GetByIdAndUserIdAsync(addressId, userId, cancellationToken);

            if (address is null)
            {
                _logger.LogWarning("Set default failed. Address not found. UserId={UserId}, AddressId={AddressId}", userId, addressId);
                throw new NotFoundException("Address not found.");
            }

            if (address.IsDefault)
                return;
  
            var currentDefaultAddress = await _userAddressRepository.GetDefaultByUserIdAsync(userId, cancellationToken);

            if (currentDefaultAddress is not null && currentDefaultAddress.Id != address.Id)
            {
                currentDefaultAddress.IsDefault = false;
                _userAddressRepository.Update(currentDefaultAddress);
            }

            address.IsDefault = true;
            _userAddressRepository.Update(address);

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Default address updated. UserId={UserId}, AddressId={AddressId}", userId, addressId);
        }


        public async Task<UserAddressDto> UpdateAsync(string userId, long addressId, UpdateUserAddressDto request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            validationResult.ThrowIfInvalid();

            var address = await _userAddressRepository.GetByIdAndUserIdAsync(addressId, userId, cancellationToken);

            if (address is null)
            {
                _logger.LogWarning("Update failed. Address not found. UserId={UserId}, AddressId={AddressId}", userId, addressId);
                throw new NotFoundException("Address not found.");
            }

            if (request.IsDefault && !address.IsDefault)
            {
                var currentDefaultAddress = await _userAddressRepository.GetDefaultByUserIdAsync(userId, cancellationToken);

                if (currentDefaultAddress is not null && currentDefaultAddress.Id != address.Id)
                {
                    currentDefaultAddress.IsDefault = false;
                    _userAddressRepository.Update(currentDefaultAddress);
                }
            }

            _mapper.Map(request, address);

            _userAddressRepository.Update(address);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Address updated. UserId={UserId}, AddressId={AddressId}, IsDefault={IsDefault}",
                userId, addressId, address.IsDefault);

            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task DeleteAsync(string userId, long addressId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var address = await _userAddressRepository.GetByIdAndUserIdAsync(addressId, userId, cancellationToken);

            if (address is null)
            {
                _logger.LogWarning("Delete failed. Address not found. UserId={UserId}, AddressId={AddressId}", userId, addressId);
                throw new NotFoundException("Address not found.");
            }

            var wasDefault = address.IsDefault;

            if (wasDefault)
            {
                var remainingAddresses = await _userAddressRepository.GetByUserIdAsync(userId, cancellationToken);

                var newDefaultAddress = remainingAddresses
                    .Where(x => x.Id != addressId)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefault();

                if (newDefaultAddress is not null)
                {
                    address.IsDefault = false;
                    _userAddressRepository.Update(address);

                    newDefaultAddress.IsDefault = true;
                    _userAddressRepository.Update(newDefaultAddress);
                }
            }

            _userAddressRepository.Delete(address);

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Address deleted. UserId={UserId}, AddressId={AddressId}", userId, addressId);
        }

    }
}
