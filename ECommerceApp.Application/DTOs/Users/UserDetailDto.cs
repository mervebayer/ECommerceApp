using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Users
{
    public record UserDetailDto(string Id, string UserName, string Email, string FirstName, string LastName, IList<string> Roles);
}

