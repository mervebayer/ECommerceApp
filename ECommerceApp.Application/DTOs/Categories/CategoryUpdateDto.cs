using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Categories
{
    public class CategoryUpdateDto
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
