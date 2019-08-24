using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingApi.Shared.Models
{
    public class Warehouse
    {
        public int DailyCapacity { get; set; }
        public List<ProductCapacity> ProductCapacities { get; set; }
    }
    public class ProductCapacity
    {
        public Guid ProductId { get; set; }
        public int Capacity { get; set; }
    }
}
