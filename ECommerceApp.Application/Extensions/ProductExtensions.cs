using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Extensions
{
    public static class ProductExtensions
    {
        public static IQueryable<Product> SortBy(this IQueryable<Product> query, ProductSortType sortType)
        {
            switch (sortType)
            {
                case ProductSortType.Newest:
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;
                case ProductSortType.PriceDesc:
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case ProductSortType.PriceAsc:
                    query = query.OrderBy(x => x.Price);
                    break;
                case ProductSortType.NameAsc:
                    query = query.OrderBy(x => x.Name);
                    break;
                default:
                    query = query.OrderByDescending(x => x.CreatedDate);
                    break;

            }
            return query;
        }
    }
}
