namespace ShoppingApi.Shared.Basket
{
    using ShoppingApi.Shared.Models;
    using System;

    public class Create
    {
        public Create(Guid sessionId)
        {
            SessionId = sessionId;
        }

        public Guid SessionId { get; }
    }

    public class GetAll { }

    public class GetById
    {
        public GetById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class AddProduct
    {
        public AddProduct(Guid sessionId, Product product)
        {
            SessionId = sessionId;
            Product = product;
        }

        public Guid SessionId { get; set; }
        public Product Product { get; set; }
    }
    public class RemoveProduct
    {
        public RemoveProduct(Guid sessionId, Guid productId)
        {
            SessionId = sessionId;
            ProductId = productId;
        }

        public Guid SessionId { get; set; }
        public Guid ProductId { get; set; }
    }

    public class Purchase
    {
        public Purchase(Guid sessionId)
        {
            SessionId = sessionId;
        }
        public Guid SessionId { get; set; }
    }
}
