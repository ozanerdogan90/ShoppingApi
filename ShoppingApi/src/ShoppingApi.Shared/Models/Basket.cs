using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingApi.Shared.Models
{
    public class Cart
    {
        public Cart()
        {
            Products = new List<Product>();
        }
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<Product> Products { get; set; }
    }
}
