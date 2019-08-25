using System;

namespace ShoppingApi.Shared.Exceptions
{
    public class DailyLimitOverException : Exception
    {
        public DailyLimitOverException() : base("Daily limit is excessed")
        {
        }
    }
}
