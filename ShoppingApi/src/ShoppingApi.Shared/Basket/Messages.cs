namespace ShoppingApi.Shared.Basket
{
    using ShoppingApi.Shared.Models;
    using System;
    using System.Collections.Generic;

    public class Create
    {
        public Create(Guid customerId)
        {
            CustomerId = customerId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public Guid CustomerId { get; }
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
        public AddProduct(Guid basketId, Product product)
        {
            CartId = basketId;
            Product = product;
        }

        public Guid CartId { get; set; }
        public Product Product { get; set; }
    }
    public class RemoveProduct
    {
        public RemoveProduct(Guid basketId, Guid productId)
        {
            CartId = basketId;
            ProductId = productId;
        }

        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
    }
}
