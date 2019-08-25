namespace ShoppingApi.AkkaServer.Basket
{
    using Akka.Actor;
    using Akka.Util;
    using ShoppingApi.AkkaServer.Warehouse;
    using ShoppingApi.Shared.Basket;
    using ShoppingApi.Shared.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static ShoppingApi.AkkaServer.Basket.Events;

    public class Actor : ReceiveActor
    {
        private readonly Dictionary<Guid, Cart> _baskets = new Dictionary<Guid, Cart>();
        public Actor()
        {
            var warehouseActor = Context.ActorOf<AkkaServer.Warehouse.Actor>();
            Receive<Create>(command =>
            {
                var cart = new Cart() { Id = command.SessionId };
                _baskets.Add(cart.Id, cart);
                Sender.Tell(new Status.Success(cart.Id));
            });


            Receive<GetById>(query =>
            {
                if (_baskets.TryGetValue(query.Id, out var cart))
                    Sender.Tell(new Status.Success(cart));
                else
                    Sender.Tell(new Status.Failure(new Exception($"Basket with id {query.Id} couldnt be found")));
            });

            Receive<GetAll>(query =>
                Sender.Tell(_baskets.Select(x => x.Value).ToList()));

            Receive<AddProduct>(command =>
            {
                if (!_baskets.TryGetValue(command.SessionId, out var cart))
                    Sender.Tell(new Status.Failure(new Exception($"Basket with id {command.SessionId} couldnt be found")));

                var product = cart.Products.FirstOrDefault(x => x.Id == command.Product.Id);
                if (product != null) product.Quantity += command.Product.Quantity;
                else cart.Products.Add(command.Product);

                Sender.Tell(new Status.Success(true));
            });

            Receive<RemoveProduct>(command =>
            {
                if (!_baskets.TryGetValue(command.SessionId, out var cart))
                    Sender.Tell(new Status.Failure(new Exception($"Basket with id {command.SessionId} couldnt be found")));

                cart.Products.RemoveAll(x => x.Id == command.ProductId);

                Sender.Tell(new Status.Success(true));
            });

            Receive<Purchase>(async command =>
            {
                if (!_baskets.TryGetValue(command.SessionId, out var cart))
                    Sender.Tell(new Status.Failure(new Exception($"Basket with id {command.SessionId} couldnt be found")));

                var warehouseResult = await warehouseActor.Ask<AkkaServer.Warehouse.Events.Event>(cart);
                if (warehouseResult is AkkaServer.Warehouse.Events.Success)
                {
                    _baskets.Remove(command.SessionId);
                    Sender.Tell(new Status.Success(true));
                    return;
                }
            });
        }
    }
}
