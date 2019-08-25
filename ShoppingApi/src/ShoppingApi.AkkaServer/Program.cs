using Akka.Actor;
using Akka.Configuration;
using System;
using System.IO;

namespace ShoppingApi.AkkaServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("ShoppingApiAkkaServer", LoadConfig("akka-server.conf")))
            {
                system.ActorOf(Props.Create(() => new AkkaServer.Warehouse.Actor()), "WarehouseActor");
                system.ActorOf(Props.Create(() => new AkkaServer.Basket.Actor()), "BasketActor");

                Console.ReadLine();
            }
        }

        private static Config LoadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                string config = File.ReadAllText(configFile);
                return ConfigurationFactory.ParseString(config);
            }

            return Config.Empty;
        }
    }
}
