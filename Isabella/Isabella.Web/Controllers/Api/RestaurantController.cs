namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Isabella.Web.ServicesControllers;
    using Isabella.Common.Dtos.Resturant;

    /// <summary>
    /// Controlador para los datos del restaurante.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : Controller
    {
        private readonly RestaurantServiceController _restaurantServiceController;

        /// <summary>
        /// Constructor RestaurantController. 
        /// </summary>
        /// <param name="restaurantServiceController"></param>
        public RestaurantController(RestaurantServiceController restaurantServiceController)
        {
            this._restaurantServiceController = restaurantServiceController;
        }

        /// <summary>
        /// Cierra el restaurante.
        /// </summary>
        /// <returns></returns>
        [HttpPut("close_restaurant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> CloseRestaurantAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
                    var result = await this
                    ._restaurantServiceController.CloseRestautantAsync()
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

        /// <summary>
        /// Abre el restaurante.
        /// </summary>
        /// <returns></returns>
        [HttpPut("open_restaurant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> OpenRestaurantAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._restaurantServiceController.OpenRestaurantAsync()
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

        /// <summary>
        /// Verifica el estado del restaurante.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_state_restaurant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetStateRestaurantAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._restaurantServiceController.GetStateRestaurantAsync()
                    .ConfigureAwait(false);
                    if(result.Success)
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
