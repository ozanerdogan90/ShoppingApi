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
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public List<Product> Products { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
