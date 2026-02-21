using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<TokenDto> CreateTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<UserDto> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default);
    }
}
