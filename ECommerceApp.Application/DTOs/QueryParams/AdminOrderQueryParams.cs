using ECommerceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.QueryParams
{
    public class AdminOrderQueryParams
    {
        private const int MaxPageSize = 20;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public OrderStatus? Status { get; set; } 
        public string? OrderNumber { get; set; }
        public string? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }

}
