using ECommerceApp.Application.DTOs;
using ECommerceApp.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<TokenDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<UserDto> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default);
        Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);

    }
}
