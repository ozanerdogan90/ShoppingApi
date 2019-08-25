using Akka.Actor;
using ShoppingApi.Shared.Basket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingApi.Shared.Client
{
    
    public class Actor : ReceiveActor, ILogReceive
    {
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://ShoppingApiAkkaServer@localhost:8081/user/BasketActor");

        public Actor()
        {
            Receive<Create>(command =>
            {
                _server.Tell(command);
            });

            //  Receive<GetById>(query =>
            //  {
            //      _server.Tell(query);
            //  });

            //  Receive<GetAll>(query =>
            //      _server.Tell(query));

            //  Receive<AddProduct>(command =>
            //  {
            //      _server.Tell(command);
            //  });

            //  Receive<RemoveProduct>(command =>
            //  {
            //      _server.Tell(command);
            //  });


            //  Receive<Purchase>(command =>
            //{
            //    _server.Tell(command);
            //});
        }
    }
}
