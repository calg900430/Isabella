namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using Common.Dtos.ProductTypeCheese;
    using Repositorys.API;
    

    /// <summary>
    /// Controlador para los Productos tipos de queso.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, owner")]
    public class ProductsTypeCheeseController : Controller
    {
        private readonly IProductTypeCheeseRepositoryAPI _productTypeCheeseService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productService"></param>
        public ProductsTypeCheeseController(IProductTypeCheeseRepositoryAPI productService)
        {
            this._productTypeCheeseService = productService;
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
                var execute_get = await this._productTypeCheeseService.GetAllProductTypeCheeseAsync();
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
                var execute_get = await this._productTypeCheeseService.GetProductTypeCheeseByIdAsync(id);
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
        /// <param name="updateProductTypeCheese"></param>
        /// <returns></returns>
        [HttpPut("update_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateProductByIdAsync(UpdateProductTypeCheeseDto updateProductTypeCheese)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productTypeCheeseService.UpdateProductTypeCheeseAsync(updateProductTypeCheese);
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
        /// <param name="addProductTypeCheese"></param>
        /// <returns></returns>
        [HttpPost("add_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAddProductAsync([FromBody] AddProductTypeCheeseDto addProductTypeCheese)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productTypeCheeseService.AddProductTypeCheeseAsync(addProductTypeCheese);
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
        /// <param name="enableOrDisableProductTypeCheese"></param>
        /// <returns></returns>
        [HttpPost("enable_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostEnableOrDisableProductAsync([FromBody] EnableOrDisableProductTypeCheeseDto enableOrDisableProductTypeCheese)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productTypeCheeseService.EnableProductTypeCheeseAsync(enableOrDisableProductTypeCheese);
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
