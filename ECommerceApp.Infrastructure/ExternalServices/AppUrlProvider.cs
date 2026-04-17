using ECommerceApp.Application.Interfaces;
using ECommerceApp.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.ExternalServices
{
    public class AppUrlProvider : IAppUrlProvider
    {
        private readonly AppUrlSettings _settings;

        public AppUrlProvider(Microsoft.Extensions.Options.IOptions<AppUrlSettings> options)
        {
            _settings = options.Value;
        }

        public string GetEmailConfirmationUrl(string userId, string encodedToken)
        {
            return $"{_settings.ApiBaseUrl.TrimEnd('/')}/api/auth/confirm-email?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(encodedToken)}";
        }
    }
}
