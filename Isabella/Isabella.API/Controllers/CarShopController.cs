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
        private readonly CartShopServiceController _carShopServiceController;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="carShopServiceController"></param>
        public CarShopController(CartShopServiceController carShopServiceController)
        {
            this._carShopServiceController = carShopServiceController;
        }

        /// <summary>
        /// Agregar un nuevo pedido al carrito de compras.
        /// </summary>
        /// <param name="addProductStandardToCarShop"></param>
        /// <returns></returns>
        [HttpPost("addtocarshop/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProductToCarShopAsync([FromBody] AddProductToCartShopDto addProductStandardToCarShop)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController.AddProductsToCartShopAsync(addProductStandardToCarShop)
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
                    ._carShopServiceController.GetMyCartShopAsync(CodeIdentification)
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
        /// Agregar un nuevo agregado a un producto que está disponible en el carrito de compras.
        /// </summary>
        /// <param name="addAggregateInProduct"></param>
        /// <returns></returns>
        [HttpPost("addtocarshop/aggregate_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddAggregateInProductToCarShopAsync([FromBody] AddAggregateInProductDto addAggregateInProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController.AddAggregateInProductAsync(addAggregateInProduct)
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
        /// Actualiza la cantidad del producto.
        /// </summary>
        /// <param name="modifyQuantityProduct"></param>
        /// <returns></returns>
        [HttpPut("update/update_quantity_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateQuantityProductInCarShopAsync([FromBody] ModifyQuantityProductDto modifyQuantityProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .UpdateQuantityProductAsync(modifyQuantityProduct)
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
        /// Incrementa la cantidad del producto en un valor dado.
        /// </summary>
        /// <param name="modifyQuantityProduct"></param>
        /// <returns></returns>
        [HttpPut("update/increment_quantity_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> IncrementQuantityProductInCarShopAsync([FromBody] ModifyQuantityProductDto modifyQuantityProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .IncrementQuantityProductAsync(modifyQuantityProduct)
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
        /// Actualiza la cantidad de un agregado para un producto.
        /// </summary>
        /// <param name="modifyCantInAggregateProduct"></param>
        /// <returns></returns>
        [HttpPut("update/update_quantity_aggregate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateQuantityAggregateOfProductInCarShopAsync([FromBody] ModifyCantInAggregateProductDto modifyCantInAggregateProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .UpdateQuantityInAggregateProductAsync(modifyCantInAggregateProduct)
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
        /// Incrementa la cantidad de un agregado dado un valor.
        /// </summary>
        /// <param name="modifyCantInAggregateProduct"></param>
        /// <returns></returns>
        [HttpPut("update/increment_quantity_aggregate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> IncrementQuantityAggregateOfProductInCarShopAsync([FromBody] ModifyCantInAggregateProductDto modifyCantInAggregateProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .IncrementQuantityInAggregateProductAsync(modifyCantInAggregateProduct)
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
        /// Actualiza la subcategoria de un producto del carrito.
        /// </summary>
        /// <param name="updateSubCategoryProduct"></param>
        /// <returns></returns>
        [HttpPut("update/subcategory_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateSubCategoryOfProductInCarShopAsync([FromBody] UpdateSubCategoryProductDto updateSubCategoryProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .UpdateSubCategoryAsync(updateSubCategoryProduct)
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
        /// Elimina la subcategoria de un producto en el carrito de compras.
        /// </summary>
        /// <param name="CodeIdentification"></param>
        /// <param name="ProductCombinedId"></param>
        /// <returns></returns>
        [HttpDelete("delete/subcategory_product_in_mycarshop")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteSubCategoryInProductInCarShopAsync(Guid CodeIdentification, int ProductCombinedId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .RemoveSubCategoryAsync(CodeIdentification, ProductCombinedId)
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
        /// Elimina un producto del carro de compras del usuario.
        /// </summary>
        /// <param name="CodeIdentification"></param>
        /// <param name="ProductCombinedId"></param>
        /// <returns></returns>
        [HttpDelete("delete/product_in_mycarshop")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteProductInCarShopAsync(Guid CodeIdentification, int ProductCombinedId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .RemoveProductOfCartShopAsync(CodeIdentification, ProductCombinedId)
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
        /// Elimina un agregado de un producto.
        /// </summary>
        /// <param name="CodeIdentification"></param>
        /// <param name="ProductCombinedId"></param>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        [HttpDelete("delete/aggregate_product_in_mycarshop")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteAggregateOfProductInCarShopAsync(Guid CodeIdentification, int ProductCombinedId, int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .RemoveAggregateInProductOfCartShopAsync(CodeIdentification, ProductCombinedId, AggregateId)
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
        /// Elimina todos los productos del carrito de compras.
        /// </summary>
        /// <param name="CodeIdentification"></param>
        /// <returns></returns>
        [HttpDelete("delete/all_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteAllProductInCarShopAsync(Guid CodeIdentification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._carShopServiceController
                    .RemoveAllCarShopAsync(CodeIdentification)
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
