using ECommerceApp.Application.Interfaces;
using ECommerceApp.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Security
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtSettings _settings;

        public JwtProvider(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public string SecurityKey => _settings.SecurityKey;
        public string Issuer => _settings.Issuer;
        public string Audience => _settings.Audience;
        public int AccessTokenExpirationMinutes => _settings.AccessTokenExpiration;
        public int RefreshTokenExpirationMinutes => _settings.RefreshTokenExpiration;
    }
}
