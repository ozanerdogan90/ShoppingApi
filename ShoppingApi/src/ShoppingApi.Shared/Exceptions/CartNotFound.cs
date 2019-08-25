using System;

namespace ShoppingApi.Shared.Exceptions
{
    public class CartNotFoundException : Exception
    {
        public CartNotFoundException()
        {

        }
        public CartNotFoundException(Guid cartId) : base($"Cart with id :{cartId} couldnt be found")
        {
        }
    }
}
