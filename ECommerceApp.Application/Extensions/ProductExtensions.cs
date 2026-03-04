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
                    query = query.OrderByDescending(x => x.CreatedDate).ThenBy(x => x.Id); //ThenBy(ID) : stable ordering for pagination
                    break;
                case ProductSortType.PriceDesc:
                    query = query.OrderByDescending(x => x.Price).ThenBy(x => x.Id);
                    break;
                case ProductSortType.PriceAsc:
                    query = query.OrderBy(x => x.Price).ThenBy(x => x.Id);
                    break;
                case ProductSortType.NameAsc:
                    query = query.OrderBy(x => x.Name).ThenBy(x => x.Id);
                    break;
                default:
                    query = query.OrderByDescending(x => x.CreatedDate).ThenBy(x => x.Id);
                    break;

            }
            return query;
        }
    }
}
