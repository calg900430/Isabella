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
    using Common.Extras;
    using Common.RepositorysDtos;
    using Extras;
    using Models;
    using RepositorysModels;
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
        /// Agregar un producto standard al carrito de compras.
        /// </summary>
        /// <param name="addProductStandardToCarShop"></param>
        /// <returns></returns>
        [HttpPost("addtocarshop/productstandard")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProductStandardToCarShopAsync([FromBody] AddProductStandardToCarShopDto addProductStandardToCarShop)
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
        /// Agregar un producto special al carrito de compras.
        /// </summary>
        /// <param name="addProductSpecialToCarShop"></param>
        /// <returns></returns>
        [HttpPost("addtocarshop/productspecial")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProductSpecialToCarShopAsync([FromBody] AddProductSpecialToCarShopDto addProductSpecialToCarShop)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController.AddProductsToCarShop(addProductSpecialToCarShop)
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
