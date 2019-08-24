using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingApi.Shared.Basket
{
    public class Events
    {
        public abstract class Event { }

        public class Created : Event
        {
            public readonly Guid Id;

            public Created(Guid cartId)
            {
                this.Id = cartId;
            }
        }

        public class NotFound : Event { }

        public class Success : Event { }
        public class Fail : Event { }
    }
}
