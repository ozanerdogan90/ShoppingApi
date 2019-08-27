using Akka.Actor;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using ShoppingApi.Shared.Basket;
using ShoppingApi.Shared.Exceptions;
using ShoppingApi.Shared.Models;
using System;
using Xunit;
using BasketActor = ShoppingApi.AkkaServer.Basket.Actor;

namespace ShoppingApi.AkkaServer.Tests.Basket
{
    public class BasketActorTests : TestKit
    {
        public BasketActorTests() : base("akka.suppress-json-serializer-warning = on") { }

        [Fact]
        public void Create_WhenCartCreated_ReturnsSuccessWithId()
        {
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Create(Guid.NewGuid())
            );

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Success>();
            expected.As<Status.Success>().Status.As<Guid>().Should().NotBeEmpty();
        }


        [Fact]
        public void GetById_WhenCartDoesntExist_ReturnsFailure()
        {
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new GetById(Guid.NewGuid())
            );

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Failure>();
            expected.As<Status.Failure>().Cause.Should().BeOfType<CartNotFoundException>();
        }

        [Fact]
        public void GetById_WhenSuccess_ReturnsCart()
        {
            var cartId = Guid.NewGuid();
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Create(cartId)
            );

            actorUnderTest.Tell(new GetById(cartId));
            var expectedCreate = ExpectMsg<Status>(TimeSpan.FromSeconds(5));

            expectedCreate.Should().BeOfType<Status.Success>();
            expectedCreate.As<Status.Success>().Status.As<Guid>().Should().NotBeEmpty();

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Success>();
            expected.As<Status.Success>().Status.As<Cart>().Should().NotBeNull();
            expected.As<Status.Success>().Status.As<Cart>().Id.Should().Be(cartId);
        }

        [Fact]
        public void AddProduct_WhenSuccess_ReturnsTrue()
        {
            var cartId = Guid.NewGuid();
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Create(cartId)
            );

            actorUnderTest.Tell(
              new AddProduct(cartId, new Product()
              {
                  Id = Guid.Parse("c1972fbd-72cc-40aa-aa07-769c7f01f300")
              })
          );

            var expectedCreate = ExpectMsg<Status>(TimeSpan.FromSeconds(5));

            expectedCreate.Should().BeOfType<Status.Success>();
            expectedCreate.As<Status.Success>().Status.As<Guid>().Should().NotBeEmpty();

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Success>();
            expected.As<Status.Success>().Status.As<bool>().Should().BeTrue();
        }

        [Fact]
        public void AddProduct_WhenDailyLimitIsOver_ReturnsFailure()
        {
            var cartId = Guid.NewGuid();
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Create(cartId)
            );

            actorUnderTest.Tell(
              new AddProduct(cartId, new Product()
              {
                  Id = Guid.Parse("c1972fbd-72cc-40aa-aa07-769c7f01f300"),
                  Quantity = 100
              })
          );

            var expectedCreate = ExpectMsg<Status>(TimeSpan.FromSeconds(5));

            expectedCreate.Should().BeOfType<Status.Success>();
            expectedCreate.As<Status.Success>().Status.As<Guid>().Should().NotBeEmpty();

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Failure>();
            expected.As<Status.Failure>().Cause.Should().BeOfType<DailyLimitOverException>();
        }

        [Fact]
        public void AddProduct_WhenProductStockExcessed_ReturnsFailure()
        {
            var cartId = Guid.NewGuid();
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Create(cartId)
            );

            actorUnderTest.Tell(
              new AddProduct(cartId, new Product()
              {
                  Id = Guid.Parse("c1972fbd-72cc-40aa-aa07-769c7f01f300"),
                  Quantity = 6
              })
          );

            var expectedCreate = ExpectMsg<Status>(TimeSpan.FromSeconds(5));

            expectedCreate.Should().BeOfType<Status.Success>();
            expectedCreate.As<Status.Success>().Status.As<Guid>().Should().NotBeEmpty();

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Failure>();
            expected.As<Status.Failure>().Cause.Should().BeOfType<ProductStockExcessException>();
        }

        [Fact]
        public void AddProduct_WhenProductDoesntExist_ReturnsFailure()
        {
            var cartId = Guid.NewGuid();
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Create(cartId)
            );

            actorUnderTest.Tell(
              new AddProduct(cartId, new Product()
              {
                  Id = Guid.Parse("c1972fbd-72cc-40aa-aa07-769c7f01f301"),
                  Quantity = 1
              })
          );

            var expectedCreate = ExpectMsg<Status>(TimeSpan.FromSeconds(5));

            expectedCreate.Should().BeOfType<Status.Success>();
            expectedCreate.As<Status.Success>().Status.As<Guid>().Should().NotBeEmpty();

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Failure>();
            expected.As<Status.Failure>().Cause.Should().BeOfType<ArgumentException>();
        }


        [Fact]
        public void RemoveProduct_WhenCartDoesntExist_ReturnsFailure()
        {
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new RemoveProduct(Guid.NewGuid(), Guid.NewGuid())
            );

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Failure>();
            expected.As<Status.Failure>().Cause.Should().BeOfType<CartNotFoundException>();
        }

        [Fact]
        public void RemoveProduct_WhenSuccess_ReturnsFailure()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.Parse("c1972fbd-72cc-40aa-aa07-769c7f01f300");
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Create(cartId)
            );
            ExpectMsg<Status>(TimeSpan.FromSeconds(5));

            actorUnderTest.Tell(
              new AddProduct(cartId, new Product()
              {
                  Id = productId
              })
          );
            ExpectMsg<Status>(TimeSpan.FromSeconds(5));

            actorUnderTest.Tell(
        new RemoveProduct(cartId, productId)
        );
            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Success>();
            expected.As<Status.Success>().Status.As<bool>().Should().BeTrue();
        }


        [Fact]
        public void Purchase_WhenCartDoesntExist_ReturnsFailure()
        {
            var actorUnderTest = ActorOfAsTestActorRef<BasketActor>();

            actorUnderTest.Tell(
                new Purchase(Guid.NewGuid())
            );

            var expected = ExpectMsg<Status>(TimeSpan.FromSeconds(5));
            expected.Should().BeOfType<Status.Failure>();
            expected.As<Status.Failure>().Cause.Should().BeOfType<CartNotFoundException>();
        }
    }
}
