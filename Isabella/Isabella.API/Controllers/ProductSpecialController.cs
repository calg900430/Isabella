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
    using Common.Dtos.CategoryProductSpecial;
    using Common.Dtos.ProductSpecial;
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
    public class ProductSpecialController : Controller
    {
        private readonly ProductSpecialServiceController _productSpecialService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productSpecialService"></param>
        public ProductSpecialController(ProductSpecialServiceController productSpecialService)
        {
            this._productSpecialService = productSpecialService;
        }

        /// <summary>
        /// Agregar un nuevo producto.
        /// </summary>
        /// <param name="addProductSpecial"></param>
        /// <returns></returns>
        [HttpPost("add/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddProductSpecialAsync([FromBody] AddProductStandardSpecialDto addProductSpecial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .AddProductSpecialAsync(addProductSpecial)
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
        /// <param name="updateProductSpecial"></param>
        /// <returns></returns>
        [HttpPut("update/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateProductSpecialdAsync(UpdateProductSpecialDto updateProductSpecial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .UpdateProductSpecialAsync(updateProductSpecial)
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
        public async Task<IActionResult> GetProductSpecialForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetProductSpecialForIdAsync(Id)
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
        public async Task<IActionResult> GetAllProductSpecialAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetAllProductSpecialAsync()
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
        public async Task<IActionResult> GetIdOfLastProductSpecialAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetIdOfLastProductSpecialAsync()
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
        public async Task<IActionResult> GetImageProductSpecialAsync(int ProductId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetImageProductSpecialForIdAsync(ProductId, ImageId)
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
        public async Task<IActionResult> AddImageProductSpecialAsync(IFormFile formFile, int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .AddImageProductSpecialAsync(formFile, ProductId)
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
        /// <param name="addImageProductSpecial"></param>
        /// <returns></returns>
        [HttpPost("addimagesdto/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddImageProductSpecialAsync(AddImageProductSpecialDto addImageProductSpecial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .AddImageProductSpecialAsync(addImageProductSpecial)
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
        public async Task<IActionResult> DeleteImageProductSpecialAsync(int ProductId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .DeleteImageProductSpecialAsync(ProductId, ImageId)
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
        public async Task<IActionResult> EnableProductSpecialAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .EnableProductSpecialAsync(ProductId)
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
        public async Task<IActionResult> DisableProductSpecialAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .DisableProductSpecialAsync(ProductId)
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
        /// <param name="CantProducts"></param>
        /// <returns></returns>
        [HttpGet("get/cant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantProductSpecialAsync(int ProductId, int CantProducts)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetCantProductSpecialAsync(ProductId, CantProducts)
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
        public async Task<IActionResult> GetListIdOfImageProductSpecialAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetListIdOfImageProductSpecialAsync(ProductId)
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
        public async Task<IActionResult> GetAllImageProductSpecialAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetAllImageProductSpecialAsync(ProductId)
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
        public async Task<IActionResult> GetCantImageProductSpecialAsync(int ProductId, int ImageId, int CantImages)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productSpecialService
                    .GetCantImageProductSpecialAsync(ProductId, ImageId, CantImages)
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
