using ECommerceApp.Core.Configurations;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using ECommerceApp.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;

        }
        public async Task<TokenDto> CreateTokenAsync(AppUser appUser, IList<string> roles)
        {
            var claims = GetClaims(appUser, roles.ToList());

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiration),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            var refreshToken = CreateRefreshToken();

            return new TokenDto
            {
                AccessToken = accessToken,
                AccessTokenExpiration = tokenDescriptor.ValidTo,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpiration)
            };
        }
        private IEnumerable<Claim> GetClaims(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            if (!string.IsNullOrWhiteSpace(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }
        private string CreateRefreshToken()
        {
            var number = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(number);
            return Convert.ToBase64String(number);
        }

    }
}
