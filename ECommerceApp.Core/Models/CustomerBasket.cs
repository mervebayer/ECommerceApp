using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Models
{
    public class CustomerBasket
    {
        public string Id { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
        public CustomerBasket()
        {
        }
        public CustomerBasket(string id)
        {
            Id = id;
            Items = new List<BasketItem>();
        }

    }
}
