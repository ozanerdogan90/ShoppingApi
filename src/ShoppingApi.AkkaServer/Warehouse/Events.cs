using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingApi.AkkaServer.Warehouse
{
    public class Events
    {
        public abstract class Event { }
        public class DailyLimitOver : Event { }
        public class InsufficientProduct : Event { }
        public class ProductNotFound : Event { }
        public class Success : Event { }
    }
}
