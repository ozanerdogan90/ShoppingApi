using Halcyon.HAL;
using Halcyon.Web.HAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingApi.Tools;
using System;
using System.Threading.Tasks;

namespace ShoppingApi.ShoppingCart
{
    [Route("api/shopping-carts")]
    [ApiController]
    public class ShoppingController : Controller
    {
        private readonly IService _service;
        public ShoppingController(IService service)
        {
            _service = service;
        }

        [HttpPost()]
        public async Task<IActionResult> Create()
        {
            var result = await _service.Create();

            return this.HAL(new { id = result }, new Link[] {
            new Link("self", $"api/shopping-carts/{result}"),
        });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([NotEmptyGuid]Guid id)
        {
            var result = await _service.GetById(id);
            return this.HAL(result, new Link[] {
            new Link("self", $"api/shopping-carts/{result.Id}"),
              });
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AddItem([NotEmptyGuid]Guid id, [BindRequired, FromBody] ProductDTO product)
        {
            await _service.AddItem(id, product);
            return Ok();
        }

        [HttpDelete("{id}/products/{productId}")]
        public async Task<IActionResult> DeleteItem([NotEmptyGuid]Guid id, [NotEmptyGuid] Guid productId)
        {
            await _service.DeleteItem(id, productId);
            return Ok();
        }


        [HttpPost("{id}/purchase")]
        public async Task<IActionResult> Purchase([NotEmptyGuid]Guid id)
        {
            //// will skip payment and other validations
            await _service.Purchase(id);
            return Ok();
        }

    }
}
