namespace Isabella.Web.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Common.Dtos.Order;
    using Repositorys.API;

    /// <summary>
    /// Controlador CarShop.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CarShopController : Controller
    {
        private readonly IOrderRepositoryAPI _orderService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderService"></param>
        public CarShopController(IOrderRepositoryAPI orderService)
        {
            this._orderService = orderService;
        }

        //GET
        /// <summary>
        /// Obtiene al carrito de compras del usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCarShopAsync(string UserName)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._orderService.GetCarsShopAsync(UserName);
                if (execute_get.Data != null)
                return Ok(execute_get); //200
                else
                return NotFound(execute_get); //404
            }
            else
            return BadRequest(); //400
        }

        //Delete
        /// <summary>
        /// Elimina un producto del carrito de compras del usuario.
        /// </summary>
        /// <param name="deleteProductToCarShop"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteCarShopAsync(DelProductToCarShopDto deleteProductToCarShop)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._orderService.DeleteProductToCarShopAsync(deleteProductToCarShop);
                if (execute_get.Data == true)
                return Ok(execute_get); //200
                else
                return NotFound(execute_get); //404
            }
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Agrega un producto al carrito de compras del usuario.
        /// </summary>
        /// <param name="addProductToCarShopDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAddProductToCarShopAsync([FromBody] AddProductToCarShopDto addProductToCarShopDto)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._orderService.AddProductToCarShopAsync(addProductToCarShopDto);
                if (execute_get.Data == true)
                return Ok(execute_get); //200
                else
                return NotFound(execute_get); //404
            }
            else
            return BadRequest(); //400
        }

        /// <summary>
        /// Actualiza el carrito de compras del usuario.
        /// </summary>
        /// <param name="updateCarShop"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutCarShopAsync([FromBody] UpdateCarShopDto updateCarShop)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._orderService.UpdateCarShopAsync(updateCarShop);
                if (execute_get.Data == true)
                return Ok(execute_get); //200
                else
                return NotFound(execute_get); //404
            }
            else
            return BadRequest(); //400
        }
    }
}
