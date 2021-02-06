namespace Isabella.Web.Controllers.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Common.Dtos.Order;
    using Repositorys.API;

    /// <summary>
    /// Controlador Order
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IOrderRepositoryAPI _orderService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderService"></param>
        public OrdersController(IOrderRepositoryAPI orderService)
        {
            this._orderService = orderService;
        }

        //GET
        /// <summary>
        /// Obtiene todos los pedidos del usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [HttpGet("get_all_orders/{UserName}")]
        [ProducesResponseType(200, Type = typeof(List<GetCarShopDto>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllOrderOfUserAsync([FromRoute] string UserName)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._orderService.GetAllOrderOfUserAsync(UserName);
                if (execute_get.Data != null)
                return Ok(execute_get); //200
                else
                return NotFound(execute_get); //404
            }
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Confirma el pedido del usuario.
        /// </summary>
        /// <param name="confirmOrderDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostConfirmAllOrdersOfUserAsync([FromBody] AddConfirmOrderDto confirmOrderDto)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._orderService.ConfirmAllOrdersOfUserAsync(confirmOrderDto);
                if (execute_get.Data != false)
                return Ok(execute_get); //200
                else
                return NotFound(execute_get); //404
            }
            else
            return BadRequest(); //400
        }
    }
}
