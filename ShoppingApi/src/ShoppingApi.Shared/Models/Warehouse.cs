using System;
using System.Collections.Generic;

namespace ShoppingApi.Shared.Models
{
    public class Warehouse
    {
        public int DailyCapacity { get; set; }
        public List<ProductCapacity> ProductCapacities { get; set; }

        //// dummy data to test
        public static Lazy<Warehouse> Instance = new Lazy<Warehouse>(new Warehouse()
        {
            DailyCapacity = 10,
            ProductCapacities = new List<ProductCapacity>() {
            new ProductCapacity(){
                ProductId=Guid.Parse("c1972fbd-72cc-40aa-aa07-769c7f01f300"),
                Capacity=5
            }
        }
        });
    }
    public class ProductCapacity
    {
        public Guid ProductId { get; set; }
        public int Capacity { get; set; }
    }
}
