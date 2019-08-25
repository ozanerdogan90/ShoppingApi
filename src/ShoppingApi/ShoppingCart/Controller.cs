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

        /// <summary>
        /// Creates a new shopping cart
        /// </summary>
        /// <returns>A shopping cart Id and resource links</returns>
        /// <response code="200">Success</response>
        /// <response code="500">If something goes wrong</response>  
        [HttpPost()]
        public async Task<IActionResult> Create()
        {
            var result = await _service.Create();

            return this.HAL(new { id = result }, new Link[] {
            new Link("self", $"api/shopping-carts/{result}"),
        });
        }

        /// <summary>
        /// Gets cart by id 
        /// </summary>
        /// <param name="id">Shopping cart id</param>
        /// <returns>Cart details</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Invalid id</response>
        /// <response code="500">If something goes wrong</response>  
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([NotEmptyGuid]Guid id)
        {
            var result = await _service.GetById(id);
            return this.HAL(result, new Link[] {
            new Link("self", $"api/shopping-carts/{result.Id}"),
              });
        }

        /// <summary>
        /// Adds new item to existing cart
        /// </summary>
        /// <param name="id">Cart id</param>
        /// <param name="product">Product details</param>
        /// <response code="200">Success</response>
        /// <response code="400">Shopping cart or product doesnt exist in system</response>
        /// <response code="404">Invalid id or product model</response>
        /// <response code="500">If something goes wrong</response>  
        [HttpPost("{id}")]
        public async Task<IActionResult> AddItem([NotEmptyGuid]Guid id, [BindRequired, FromBody] ProductDTO product)
        {
            await _service.AddItem(id, product);
            return Ok();
        }

        /// <summary>
        /// Deletes item from shopping cart
        /// </summary>
        /// <param name="id">Cart id</param>
        /// <param name="productId">Product id</param>
        /// <response code="200">Success</response>
        /// <response code="404">Invalid id or product model</response>
        /// <response code="500">If something goes wrong</response>  
        [HttpDelete("{id}/products/{productId}")]
        public async Task<IActionResult> DeleteItem([NotEmptyGuid]Guid id, [NotEmptyGuid] Guid productId)
        {
            await _service.DeleteItem(id, productId);
            return Ok();
        }


        /// <summary>
        /// Purchases items
        /// </summary>
        /// <param name="id">Cart id</param>
        /// <response code="200">Success</response>
        /// <response code="404">Invalid id</response>
        /// <response code="500">If something goes wrong</response> 
        [HttpPost("{id}/purchase")]
        public async Task<IActionResult> Purchase([NotEmptyGuid]Guid id)
        {
            //// will skip payment and other validations
            await _service.Purchase(id);
            return Ok();
        }

    }
}
