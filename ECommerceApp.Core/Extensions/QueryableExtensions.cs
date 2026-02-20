using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ToPagedList<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 20 : pageSize;

            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
