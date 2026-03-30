using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.UserProfiles
{
    public sealed record UserProfileDto(string Id, string UserName, string Email, string FirstName, string LastName, string PhoneNumber);
}
