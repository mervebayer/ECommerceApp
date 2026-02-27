using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.QueryParams
{
    public class ProductQueryParams
    {
        public string? Search {  get; set; }
        public long? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductSortType SortType { get; set; } = ProductSortType.Newest;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

    }
}
