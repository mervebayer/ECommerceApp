using ECommerceApp.Application.DTOs;
using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> CreateTokenAsync(AppUser appUser, IList<string> roles);
    }
}
