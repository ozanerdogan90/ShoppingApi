using Microsoft.AspNetCore.Mvc;
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
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] ProductDTO product)
        {
            await _service.AddItem(id, product);
            return Ok();
        }

        [HttpDelete("{id}/products/{productId}")]
        public async Task<IActionResult> DeleteItem(Guid id, Guid productId)
        {
            await _service.DeleteItem(id, productId);
            return Ok();
        }


        [HttpPost("{id}/purchase")]
        public async Task<IActionResult> Purchase(Guid id)
        {
            //// will skip payment and other validations
            await _service.Purchase(id);
            return Ok();
        }

    }
}
