using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.BasketModule
{
    public class BasketItem
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public decimal price { get; set; }
        public decimal quantity { get; set; }
    }
}
