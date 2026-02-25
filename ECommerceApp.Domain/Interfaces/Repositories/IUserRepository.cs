using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{ 
    public interface IUserRepository
    {
        Task<AppUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
