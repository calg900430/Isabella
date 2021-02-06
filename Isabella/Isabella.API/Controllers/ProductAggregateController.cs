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
    using Common.Dtos.ProductAggregate;
    using Common.Extras;
    using Common.RepositorysDtos;
    using Extras;
    using Models;
    using RepositorysModels;
    using ServicesControllers;
   
    /// <summary>
    /// Controlador para los productos special.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductAggregateController : Controller
    {
        private readonly ProductAggregateServiceController _productAggregateService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productAggregateService"></param>
        public ProductAggregateController(ProductAggregateServiceController productAggregateService)
        {
            this._productAggregateService = productAggregateService;
        }

        /// <summary>
        /// Agregar un nuevo producto.
        /// </summary>
        /// <param name="addProductAggregate"></param>
        /// <returns></returns>
        [HttpPost("add/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddProductAggregateAsync([FromBody] AddProductAggregateDto addProductAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .AddProductAggregateAsync(addProductAggregate)
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
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProductAggregate"></param>
        /// <returns></returns>
        [HttpPut("update/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateProductAggregatedAsync(UpdateProductAggregateDto updateProductAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .UpdateProductAggregateAsync(updateProductAggregate)
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
        /// Obtiene un producto dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("get/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProductAggregateForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetProductAggregateForIdAsync(Id)
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
        /// Obtiene todos los productos disponibles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductAggregateAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetAllProductAggregateAsync()
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
        /// Obtiene el Id del último producto que se registró en el sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/id_last_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetIdOfLastProductAggregateAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetIdOfLastProductAggregateAsync()
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
        /// Accede una imagen especifica de un producto.
        /// </summary>
        /// <returns></returns>
        [HttpGet("getimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetImageProductAggregateAsync(int ProductId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetImageProductAggregateForIdAsync(ProductId, ImageId)
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
        /// Agrega una imagen de un producto(Usando IFormFile).
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpPost("addimagesformfile/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddImageProductAggregateAsync(IFormFile formFile, int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .AddImageProductAggregateAsync(formFile, ProductId)
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
        /// Agrega una imagen a un producto(Usando Dto).
        /// </summary>
        /// <param name="addImageProductAggregate"></param>
        /// <returns></returns>
        [HttpPost("addimagesdto/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddImageProductAggregateAsync(AddImageProductAggregateDto addImageProductAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .AddImageProductAggregateAsync(addImageProductAggregate)
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
        /// Borra una imagen de un producto.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("deleteimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteImageProductAggregateAsync(int ProductId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .DeleteImageProductAggregateAsync(ProductId, ImageId)
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
        /// Pone un producto en disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpPost("enable_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> EnableProductAggregateAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .EnableProductAggregateAsync(ProductId)
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
        /// Pone un producto en no disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpPost("disable_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DisableProductAggregateAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .DisableProductAggregateAsync(ProductId)
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
        /// Obtiene una cantidad determinada de productos dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="CantProduct"></param>
        /// <returns></returns>
        [HttpGet("get/cant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantProductAggregateAsync(int ProductId, int CantProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetCantProductAggregateAsync(ProductId, CantProduct)
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
        /// Obtiene todos los Id de las imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpGet("get/all_listid_images")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetListIdOfImageProductAggregateAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetListIdOfImageProductAggregateAsync(ProductId)
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
        /// Obtiene todas las imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpGet("getallimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllImageProductAggregateAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetAllImageProductAggregateAsync(ProductId)
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
        /// Obtiene una cantidad especifica de imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <param name="CantImages"></param>
        /// <returns></returns>
        [HttpGet("getcantimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantImageProductAggregateAsync(int ProductId, int ImageId, int CantImages)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productAggregateService
                    .GetCantImageProductAggregateAsync(ProductId, ImageId, CantImages)
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
