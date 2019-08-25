namespace ShoppingApi.AkkaServer.Basket
{
    using Akka.Actor;
    using ShoppingApi.AkkaServer.Warehouse;
    using ShoppingApi.Shared.Basket;
    using ShoppingApi.Shared.Exceptions;
    using ShoppingApi.Shared.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                    Sender.Tell(new Status.Failure(new CartNotFoundException(query.Id)));
            });

            Receive<GetAll>(query =>
                Sender.Tell(_baskets.Select(x => x.Value).ToList()));

            Receive<AddProduct>(async command =>
            {
                if (!_baskets.TryGetValue(command.SessionId, out var cart))
                {
                    Sender.Tell(new Status.Failure(new CartNotFoundException(command.SessionId)));
                    return;
                }

                var sender = Context.Sender;
                var warehouseResult = await warehouseActor.Ask<Events.Event>(command.Product);
                if (warehouseResult is Events.Success)
                {
                    var product = cart.Products.FirstOrDefault(x => x.Id == command.Product.Id);
                    if (product != null) product.Quantity += command.Product.Quantity;
                    else cart.Products.Add(command.Product);
                    sender.Tell(new Status.Success(true));
                }
                else
                {
                    sender.Tell(ConvertFailureToException(warehouseResult));
                }
            });

            Receive<RemoveProduct>(command =>
            {
                if (!_baskets.TryGetValue(command.SessionId, out var cart))
                {
                    Sender.Tell(new Status.Failure(new CartNotFoundException(command.SessionId)));
                    return;
                }

                cart.Products.RemoveAll(x => x.Id == command.ProductId);

                Sender.Tell(new Status.Success(true));
            });

            Receive<Purchase>(async command =>
            {
                if (!_baskets.TryGetValue(command.SessionId, out var cart))
                {
                    Sender.Tell(new Status.Failure(new CartNotFoundException(command.SessionId)));
                    return;
                }
                var sender = Context.Sender;
                var warehouseResult = await warehouseActor.Ask<Events.Event>(cart);
                if (warehouseResult is Events.Success)
                {
                    _baskets.Remove(command.SessionId);
                    sender.Tell(new Status.Success(true));
                    return;
                }
                else
                {
                    sender.Tell(ConvertFailureToException(warehouseResult));
                }
            });
        }
        private Status ConvertFailureToException(Events.Event _event)
        {
            if (_event is Events.DailyLimitOver) return new Status.Failure(new DailyLimitOverException());
            if (_event is Events.InsufficientProduct) return new Status.Failure(new ProductStockExcessException());
            if (_event is Events.ProductNotFound) return new Status.Failure(new ArgumentException("Product is not found"));

            return new Status.Failure(new Exception());
        }
    }
}
