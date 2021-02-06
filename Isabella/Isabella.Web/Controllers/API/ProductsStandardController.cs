namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Common.Dtos.ProductStandard;
    using Repositorys.API;

    /// <summary>
    /// Controlador para Productos Standard.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, owner")]
    public class ProductsStandardController : Controller
    {
        private readonly IProductStandardRepositoryAPI _productStandardService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productService"></param>
        public ProductsStandardController(IProductStandardRepositoryAPI productService)
        {
            this._productStandardService = productService;
        }

        //GET
        /// <summary>
        /// Accede a todos los productos de este tipo que esten disponibles en el restaurante.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_products")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllProductAsync()
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productStandardService.GetAllProductStandardAsync();
                if (execute_get.Data != null)
                    return Ok(execute_get); //200
                else
                    return NotFound(execute_get); //404
            }
            else
                return BadRequest(); //400
        }

        //GET
        /// <summary>
        /// Accede a un producto del restaurante que este disponible dado su Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get_product/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productStandardService.GetProductStandardByIdAsync(id);
                if (execute_get.Data != null)
                    return Ok(execute_get); //200
                else
                    return NotFound(execute_get); //404
            }
            else
                return BadRequest(); //400
        }

        //PUT
        /// <summary>
        /// Actualiza el producto.
        /// </summary>
        /// <param name="updateProductStandard"></param>
        /// <returns></returns>
        [HttpPut("update_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateProductByIdAsync(UpdateProductStandardDto updateProductStandard)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productStandardService.UpdateProductStandardAsync(updateProductStandard);
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
        /// Agrega el producto.
        /// </summary>
        /// <param name="addProductStandard"></param>
        /// <returns></returns>
        [HttpPost("add_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAddProductAsync([FromBody] AddProductStandardDto addProductStandard)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productStandardService.AddProductStandardAsync(addProductStandard);
                if (execute_get.Data == true)
                    return Ok(execute_get); //200
                else
                    return NotFound(execute_get); //404
            }
            else
                return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Agrega una calificación a un producto del restaurante.
        /// </summary>
        /// <param name="addCalification"></param>
        /// <returns></returns>
        [HttpPost("add_calification")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAddClasificationProductAsync([FromBody] AddCalificationProductStandardDto addCalification)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productStandardService.AddCalificationForProductStandardAsync(addCalification);
                if (execute_get.Data == true)
                    return Ok(execute_get); //200
                else
                    return NotFound(execute_get); //404
            }
            else
                return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Habilita o deshabilita un producto.
        /// </summary>
        /// <param name="enableOrDisableProductStandard"></param>
        /// <returns></returns>
        [HttpPost("enable_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostEnableOrDisableProductAsync([FromBody] EnableOrDisableProductStandardDto enableOrDisableProductStandard)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productStandardService.EnableProductStandardAsync(enableOrDisableProductStandard);
                if (execute_get.Data == true)
                    return Ok(execute_get); //200
                else
                    return NotFound(execute_get); //404
            }
            else
                return BadRequest(); //400
        }
    }
}
