using System;

namespace ShoppingApi.Shared.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public double Deci { get; set; }
    }
}
