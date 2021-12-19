namespace Isabella.Web.Controllers.API
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
    using Common.Dtos.Categorie;
    using Common.Dtos.Product;
    using Common.RepositorysDtos;
    using Models;
    using ServicesControllers;
   
    /// <summary>
    /// Controlador para los productos standard.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : Controller
    {
        private readonly ProductServiceController _productRepository;
      
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productStandardRepository"></param>
        public ProductController(ProductServiceController productStandardRepository)
        {
            this._productRepository = productStandardRepository;
        }

        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="addProductStandard"></param>
        /// <returns></returns>
        [HttpPost("add/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddProductdAsync([FromBody] AddProductDto addProductStandard)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository.AddProductAsync(addProductStandard).ConfigureAwait(false);
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
        /// <param name="updateProductStandard"></param>
        /// <returns></returns>
        [HttpPut("update/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductDto updateProductStandard)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .UpdateProductAsync(updateProductStandard)
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
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProductForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetProductForIdAsync(Id)
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
        /// Obtiene todos los productos.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetAllProductAsync()
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
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetIdOfLastProductAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetIdOfLastProductAsync()
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
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetImageProductAsync(int ProductId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetImageProductForIdAsync(ProductId, ImageId)
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
        public async Task<IActionResult> AddImageProductAsync(IFormFile formFile, int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .AddImageProductAsync(formFile, ProductId)
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
        /// <param name="addImageProductStandard"></param>
        /// <returns></returns>
        [HttpPost("addimagesdto/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddImageProductAsync([FromBody] AddImageProductDto addImageProductStandard)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .AddImageProductAsync(addImageProductStandard)
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
        public async Task<IActionResult> DeleteImageProductAsync(int ProductId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .DeleteImageProductAsync(ProductId, ImageId)
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
        /// Establece un producto en disponible o no disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpPost("enable_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> EnableProductAsync(int ProductId, bool enable)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .EnableProductAsync(ProductId, enable)
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
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantProductAsync(int ProductId, int CantProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetCantProductAsync(ProductId, CantProduct)
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
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetListIdOfImageProductAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetListIdOfImageProductAsync(ProductId)
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
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllImageProductAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetAllImageProductAsync(ProductId)
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
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantImageProductAsync(int ProductId, int ImageId, int CantImages)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetCantImageProductAsync(ProductId, ImageId, CantImages)
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
        /// Obtiene un producto dado su Id si el mismo está disponible.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("get/id_available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProductIsAvailableForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetProductIsAvailableForIdAsync(Id)
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
        /// Obtiene una cantidad determinada de productos disponibles dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="CantProduct"></param>
        /// <returns></returns>
        [HttpGet("get/cant_available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantProductIsAvailabelAsync(int ProductId, int CantProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetCantProductIsAvailableAsync(ProductId, CantProduct)
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
        [HttpGet("get/all_available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductIsAvailableAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetAllProductIsAvailableAsync()
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
        /// Obtiene todos los productos disponibles de una categoria determinada.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_productavailable_of_category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductIsAvailableOfCategory(int CategoryId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetAllProductIsAvailableOfCategory(CategoryId)
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
        /// Obtiene todos los productos de una categoria determinada.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_product_of_category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductOfCategory(int CategoryId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .GetAllProductOfCategory(CategoryId)
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
        /// Elimina un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpDelete("delete/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteProductAsync(int ProductId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._productRepository
                    .DeleteProductAsync(ProductId)
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
