using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.DTOs.Users;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Exceptions;
using ECommerceApp.Service.Interfaces;
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
        public AuthenticationService(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) throw new NotFoundException("Invalid email or password.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid) throw new BadRequestException("Invalid email or password.");

            var tokenDto = await _tokenService.CreateTokenAsync(user);

            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiration = tokenDto.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return tokenDto;
        }

        public async Task<TokenDto> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiration < DateTime.Now)
            {
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }

            var tokenDto = await _tokenService.CreateTokenAsync(user);

            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiration = tokenDto.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return tokenDto;
        }

   
    }
}
