namespace Isabella.API.Controllers
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderServiceController"></param>
        public OrderController(OrderServiceController orderServiceController)
        {
            this._orderServiceController = orderServiceController;
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
        //[ProducesResponseType(401)]
        //[ProducesResponseType(403)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult>ConfirmOrderAsync([FromBody] ConfirmOrderDto confirmOrder)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        [HttpGet("get/all_order")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAggregateForIdAsync(Guid codeIdentification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._orderServiceController
                    .GetAllOrderAsync(codeIdentification)
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
