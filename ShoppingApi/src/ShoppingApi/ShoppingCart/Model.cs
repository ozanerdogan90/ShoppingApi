using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.ShoppingCart
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public double Deci { get; set; }
    }

    public class CartDTO
    {
       
        public Guid Id { get; set; }
        public List<ProductDTO> Products { get; set; }
    }
}
