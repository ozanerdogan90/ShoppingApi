using Akka.Actor;

namespace ShoppingApi.Tools
{
    public static class StatusExtension
    {
        public static T To<T>(this Status status)
        {
            if (status is Status.Success success)
                return (T)success.Status;

            var exception = status as Status.Failure;
            throw exception.Cause;
        }
    }
}
