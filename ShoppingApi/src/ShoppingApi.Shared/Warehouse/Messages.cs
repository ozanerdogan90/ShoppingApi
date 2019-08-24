using ShoppingApi.Shared.Models;
using System;
using System.Collections.Generic;

namespace ShoppingApi.Shared.Warehouse
{
    public class Purchase
    {
        public Purchase(Guid cartId, List<Product> products)
        {
            Products = products;
            CartId = cartId;
        }
        public Guid CartId { get; set; }
        public List<Product> Products { get; }
    }
}
