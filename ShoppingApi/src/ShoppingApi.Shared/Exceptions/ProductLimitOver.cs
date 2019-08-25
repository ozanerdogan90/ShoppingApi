using System;

namespace ShoppingApi.Shared.Exceptions
{
    public class ProductStockExcessException : Exception
    {
        public ProductStockExcessException() : base("Product stock is excessed")
        {
        }
    }
}
