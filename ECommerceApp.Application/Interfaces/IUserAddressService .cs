using ECommerceApp.Application.DTOs.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IUserAddressService
    {
        Task<IReadOnlyList<UserAddressDto>> GetMyAddressesAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserAddressDto> GetMyAddressByIdAsync(string userId, long addressId, CancellationToken cancellationToken = default);
        Task<UserAddressDto> CreateAsync(string userId, CreateUserAddressDto request, CancellationToken cancellationToken = default);
        Task<UserAddressDto> UpdateAsync(string userId, long addressId, UpdateUserAddressDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, long addressId, CancellationToken cancellationToken = default);
        Task SetDefaultAsync(string userId, long addressId, CancellationToken cancellationToken = default);

    }
}
