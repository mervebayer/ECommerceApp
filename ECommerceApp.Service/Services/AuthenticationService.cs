using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.DTOs.Users;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Exceptions;
using ECommerceApp.Data;
using ECommerceApp.Service.Extensions;
using ECommerceApp.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IValidator<UserRegisterDto> _registerValidator;
        private readonly AppDbContext _context;
        public AuthenticationService(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper, IValidator<UserRegisterDto> registerValidator, AppDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _registerValidator = registerValidator;
            _context = context;
        }
        public async Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail) ?? await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);
            if (user == null)      
                throw new UnauthorizedException("Invalid username/email or password.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid) throw new BadRequestException("Invalid username/email or password.");

            var roles = await _userManager.GetRolesAsync(user);

            cancellationToken.ThrowIfCancellationRequested();

            var tokenDto = await _tokenService.CreateTokenAsync(user, roles);

            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiration = tokenDto.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return tokenDto;
        }

        public async Task<TokenDto> CreateTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

            if (user == null || user.RefreshTokenExpiration < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var tokenDto = await _tokenService.CreateTokenAsync(user, roles);

            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiration = tokenDto.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return tokenDto;
        }

        // TODO: change BadRequest to ValidationException 
        public async Task<UserDto> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto, cancellationToken);
            validationResult.ThrowIfInvalid();

            var userExists = await _userManager.FindByNameAsync(registerDto.UserName);
            if (userExists != null)
                throw new BadRequestException("UserName already taken.");

            if (!string.IsNullOrWhiteSpace(registerDto.Email))
            {
                var emailExists = await _userManager.FindByEmailAsync(registerDto.Email);
                if (emailExists != null)
                    throw new BadRequestException("Email already taken.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {

                var user = _mapper.Map<AppUser>(registerDto);

                var createResult = await _userManager.CreateAsync(user, registerDto.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Registration failed: {errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Authorization error: {errors}");
                }
                await transaction.CommitAsync(cancellationToken);

                return _mapper.Map<UserDto>(user);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                if (ex is BadRequestException) throw;

                throw new BadRequestException("A technical error occurred during the registration process.");
            }         
        }
    }
}
