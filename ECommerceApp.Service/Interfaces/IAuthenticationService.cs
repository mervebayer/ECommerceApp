using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenDto> LoginAsync(LoginDto loginDto);
        Task<TokenDto> CreateTokenByRefreshTokenAsync(string refreshToken);
        
    }
}
