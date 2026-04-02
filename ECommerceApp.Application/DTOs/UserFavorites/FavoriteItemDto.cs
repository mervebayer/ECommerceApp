using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.UserFavorites
{
    public sealed record FavoriteItemDto(long ProductId, string ProductName, decimal Price, string ImageUrl);
}
