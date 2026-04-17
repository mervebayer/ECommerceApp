using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces
{
    public interface IAppUrlProvider
    {
        string GetEmailConfirmationUrl(string userId, string encodedToken);
    }
}
