using ECommerceApp.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Common.Helpers
{
    public static class RefreshTokenHasher
    {
        public static string Hash(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new BadRequestException("Refresh token is required.");

            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
