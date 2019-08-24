namespace ShoppingApi.Shared.Warehouse
{
    using Akka.Actor;
    using System.Linq;
    using static ShoppingApi.Shared.Warehouse.Events;
    using State = Shared.Models.Warehouse;
    public class Actor : ReceiveActor
    {
        private State _state;
        public Actor()
        {
            _state = new State();

            Receive<Purchase>(command =>
            {
                if (!(_state.DailyCapacity - command.Products.Sum(x => x.Quantity) < 0))
                {
                    Sender.Tell(new DailyLimitOver());
                    return;
                }

                foreach (var _product in command.Products)
                {
                    var product = _state.ProductCapacities.FirstOrDefault(x => x.ProductId == _product.Id);
                    if (product == null)
                    {
                        Sender.Tell(new ProductNotFound());
                        return;
                    }

                    if (product.Capacity - _product.Quantity < 0)
                    {
                        Sender.Tell(new InsufficientProduct());
                        return;
                    }

                    product.Capacity -= _product.Quantity;

                }

                _state.DailyCapacity -= command.Products.Sum(x => x.Quantity);
                Sender.Tell(new Success());
            });
        }
    }
}
