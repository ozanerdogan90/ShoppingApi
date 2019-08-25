using Akka.Actor;
using AutoMapper;
using ShoppingApi.Shared.Basket;
using ShoppingApi.Shared.Models;
using ShoppingApi.Tools;
using System;
using System.Threading.Tasks;

namespace ShoppingApi.ShoppingCart
{
    public interface IService
    {
        Task<Guid> Create();
        Task<bool> AddItem(Guid sessionId, ProductDTO product);
        Task<bool> DeleteItem(Guid cartId, Guid productId);
        Task<CartDTO> GetById(Guid sessionId);
        Task<bool> Purchase(Guid sessionId);
    }

    public class Service : IService
    {
        private readonly ActorSelection _akkaServer;
        private readonly IMapper _mapper;
        public Service(ActorSelection akkaServer, IMapper mapper)
        {
            _akkaServer = akkaServer;
            _mapper = mapper;
        }

        public async Task<Guid> Create()
        {
            var result = await _akkaServer.Ask<Status>(new Create(Guid.NewGuid()));
            return result.To<Guid>();
        }

        public async Task<bool> AddItem(Guid sessionId, ProductDTO product)
        {
            var entity = _mapper.Map<Product>(product);
            var result = await _akkaServer.Ask<Status>(new AddProduct(sessionId, entity));
            return result.To<bool>();
        }

        public async Task<bool> DeleteItem(Guid cartId, Guid productId)
        {
            var result = await _akkaServer.Ask<Status>(new RemoveProduct(cartId, productId));
            return result.To<bool>();
        }

        public async Task<CartDTO> GetById(Guid sessionId)
        {
            var entity = await _akkaServer.Ask<Status>(new GetById(sessionId));
            return _mapper.Map<CartDTO>(entity.To<Cart>());
        }

        public async Task<bool> Purchase(Guid sessionId)
        {
            var result = await _akkaServer.Ask<Status>(new Purchase(sessionId));
            return result.To<bool>();
        }
    }
}
