using AutoMapper;
using ECommerceApp.Application.DTOs.UserProfiles;
using ECommerceApp.Application.Extensions;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<UpdateUserProfileDto> _updateUserProfileValidator;
        private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
        private readonly IMapper _mapper;

        public UserProfileService(UserManager<AppUser> userManager, IValidator<UpdateUserProfileDto> updateUserProfileValidator, IValidator<ChangePasswordDto> changePasswordValidator, IMapper mapper)
        {
            _userManager = userManager;
            _updateUserProfileValidator = updateUserProfileValidator;
            _changePasswordValidator = changePasswordValidator;
            _mapper = mapper;
        }
        public async Task<UserProfileDto> GetMyProfileAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new NotFoundException("User not found.");

            return _mapper.Map<UserProfileDto>(user);
        }
        public async Task<UserProfileDto> UpdateMyProfileAsync(string userId, UpdateUserProfileDto request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validationResult = await _updateUserProfileValidator.ValidateAsync(request, cancellationToken);
            validationResult.ThrowIfInvalid();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new NotFoundException("User not found.");

            if (!string.Equals(user.UserName, request.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var existingUserByUserName = await _userManager.FindByNameAsync(request.UserName);
                if (existingUserByUserName is not null)
                    throw new BadRequestException("Username is already taken.");
            }

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingUserByEmail is not null)
                    throw new BadRequestException("Email is already taken.");
            }

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new BadRequestException($"Profile update failed: {errors}");
            }

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validationResult = await _changePasswordValidator.ValidateAsync(request, cancellationToken);
            validationResult.ThrowIfInvalid();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new NotFoundException("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new BadRequestException($"Password change failed: {errors}");
            }
        }


    }
}
