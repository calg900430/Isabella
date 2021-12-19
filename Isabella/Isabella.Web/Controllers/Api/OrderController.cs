namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Isabella.Common.Dtos.Order;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ServicesControllers;

    /// <summary>
    /// Ordenes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly OrderServiceController _orderServiceController;
        private readonly RestaurantServiceController _restaurantServiceController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderServiceController"></param>
        /// <param name="restaurantServiceController"></param>
        public OrderController(OrderServiceController orderServiceController,
        RestaurantServiceController restaurantServiceController)
        {
            this._orderServiceController = orderServiceController;
            this._restaurantServiceController = restaurantServiceController;
        }

        /// <summary>
        /// Confirma el pedido del usuario.
        /// </summary>
        /// <param name="confirmOrder"></param>
        /// <returns></returns>
        [HttpPost("confirm/order")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult>ConfirmOrderAsync([FromBody] ConfirmOrderDto confirmOrder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Obtiene los claims del usuario.
                    this._restaurantServiceController.ClaimsPrincipal = HttpContext.User;
                    //Verifica si el restaurante está abierto para vender
                    var state_restaurant = await this._restaurantServiceController
                    .GetStateRestaurantAsync().ConfigureAwait(false);
                    if (state_restaurant.Data == false)
                    return BadRequest(state_restaurant); //400
                    this._orderServiceController.ClaimsPrincipal = HttpContext.User;
                    var result = await this._orderServiceController
                    .ConfirmOrderAsync(confirmOrder).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todas las ordenes de un usuario.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_order")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAggregateForIdAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Obtiene los claims del usuario.
                    this._restaurantServiceController.ClaimsPrincipal = HttpContext.User;
                    //Verifica si el restaurante está abierto para vender
                    var state_restaurant = await this._restaurantServiceController
                    .GetStateRestaurantAsync().ConfigureAwait(false);
                    if (state_restaurant.Data == false)
                     return BadRequest(state_restaurant); //400
                    this._orderServiceController.ClaimsPrincipal = HttpContext.User;
                    var result = await this._orderServiceController
                    .GetAllOrderAsync()
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
