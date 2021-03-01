namespace Isabella.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using Common;
    using Common.Dtos.CarShop;
    using Common.RepositorysDtos;
    using Models.Entities;
    using ServicesControllers;

    /// <summary>
    /// Controlador para el carrito de compras.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CarShopController : Controller
    {
        private readonly CarShopServiceController _carShopServiceController;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="carShopServiceController"></param>
        public CarShopController(CarShopServiceController carShopServiceController)
        {
            this._carShopServiceController = carShopServiceController;
        }

        /// <summary>
        /// Agregar un nuevo pedido al carrito de compras.
        /// </summary>
        /// <param name="addProductStandardToCarShop"></param>
        /// <returns></returns>
        [HttpPost("addtocarshop/productstandard")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProductStandardToCarShopAsync([FromBody] AddProductToCarShopDto addProductStandardToCarShop)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController.AddProductsToCarShop(addProductStandardToCarShop)
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
        /// Obtiene el carrito de compras del usuario.
        /// </summary>
        /// <param name="CodeIdentification"></param>
        /// <returns></returns>
        [HttpGet("get/my_carshop")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetMyCarShopAsync(Guid CodeIdentification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController.GetMyCarShop(CodeIdentification)
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
