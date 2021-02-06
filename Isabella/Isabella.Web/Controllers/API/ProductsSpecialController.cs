namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Common.Dtos.ProductSpecial;
    using Repositorys.API;

    /// <summary>
    /// Controlador para Productos Especiales
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,
    public class ProductsSpecialController : Controller
    {
        private readonly IProductSpecialRepositoryAPI _productSpecialService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productSpecialService"></param>
        public ProductsSpecialController(IProductSpecialRepositoryAPI productSpecialService)
        {
            this._productSpecialService = productSpecialService;
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
                var execute_get = await this._productSpecialService.GetAllProductSpecialAsync();
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
                var execute_get = await this._productSpecialService.GetProductSpecialByIdAsync(id);
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
        /// <param name="updateProductSpecial"></param>
        /// <returns></returns>
        [HttpPut("update_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateProductByIdAsync(UpdateProductSpecialDto updateProductSpecial)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productSpecialService.UpdateProductSpecialAsync(updateProductSpecial);
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
        /// Agrega una calificación a un producto del restaurante.
        /// </summary>
        /// <param name="addCalification"></param>
        /// <returns></returns>
        [HttpPost("add_calification")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAddClasificationProductAsync([FromBody] AddCalificationProductSpecialDto addCalification)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productSpecialService.AddCalificationForProductSpecialAsync(addCalification);
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
        /// Agrega el producto.
        /// </summary>
        /// <param name="addProductSpecial"></param>
        /// <returns></returns>
        [HttpPost("add_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAddProductAsync([FromBody] AddProductSpecialDto addProductSpecial)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productSpecialService.AddProductSpecialAsync(addProductSpecial);
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
        /// <param name="enableOrDisableProductSpecial"></param>
        /// <returns></returns>
        [HttpPost("enable_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostEnableOrDisableProductAsync([FromBody] EnableOrDisableProductSpecialDto enableOrDisableProductSpecial)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productSpecialService.EnableProductSpecialAsync(enableOrDisableProductSpecial);
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
