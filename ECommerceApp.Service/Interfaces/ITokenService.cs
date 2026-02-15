using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Service.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> CreateTokenAsync(AppUser appUser, IList<string> roles);
    }
}
