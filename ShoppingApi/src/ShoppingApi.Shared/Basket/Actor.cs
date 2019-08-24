namespace ShoppingApi.Shared.Basket
{
    using Akka.Actor;
    using ShoppingApi.Shared.Warehouse;
    using ShoppingApi.Shared.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static ShoppingApi.Shared.Basket.Events;

    public class Actor : ReceiveActor
    {
        private readonly Dictionary<Guid, Cart> _baskets = new Dictionary<Guid, Cart>();
        public Actor()
        {
            var warehouseActor = Context.ActorOf<Shared.Warehouse.Actor>();
            Receive<Create>(command =>
            {
                var cart = new Cart() { CustomerId = command.CustomerId, Id = command.Id };
                _baskets.Add(cart.Id, cart);
                Sender.Tell(new Success());
            });

            Receive<GetById>(query =>
            {
                if (_baskets.TryGetValue(query.Id, out var cart))
                    Sender.Tell(cart);
                else
                    Sender.Tell(new NotFound());
            });

            Receive<GetAll>(query =>
                Sender.Tell(_baskets.Select(x => x.Value).ToList()));

            Receive<AddProduct>(command =>
            {
                if (!_baskets.TryGetValue(command.CartId, out var cart))
                    Sender.Tell(new NotFound());

                var product = cart.Products.FirstOrDefault(x => x.Id == command.Product.Id);
                if (product != null) product.Quantity += command.Product.Quantity;
                else cart.Products.Add(command.Product);

                Sender.Tell(new Success());
            });

            Receive<RemoveProduct>(command =>
            {
                if (!_baskets.TryGetValue(command.CartId, out var cart))
                    Sender.Tell(new NotFound());

                cart.Products.RemoveAll(x => x.Id == command.ProductId);

                Sender.Tell(new Success());
            });

            Receive<Purchase>(async command =>
            {
                var warehouseResult = await warehouseActor.Ask<Shared.Warehouse.Events.Event>(command);
                if (warehouseResult is Shared.Warehouse.Events.Success)
                {
                    _baskets.Remove(command.CartId);
                    Sender.Tell(new Success());
                    return;
                }
                Sender.Tell(new Fail());
            });
        }
    }
}
