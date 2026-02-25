using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IJwtProvider
    {
        string SecurityKey { get; }
        string Issuer { get; }
        string Audience { get; }
        int AccessTokenExpirationMinutes { get; }
        int RefreshTokenExpirationMinutes { get; }
    }
}
