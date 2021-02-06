namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Common.Dtos.Product;
    using Repositorys.API;

    /// <summary>
    /// Controlador Productos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : Controller
    {
        private readonly IProductRepositoryAPI _productService;
       
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="productService"></param>
        public ProductsController(IProductRepositoryAPI productService)
        {
            this._productService = productService;
        }

        //GET
        /// <summary>
        /// Accede a un producto determinado del sistema dado su Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(GetProductDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] Guid id)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productService.GetProductByIdAsync(id);
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
        /// Obtiene todos los productos disponibles del restaurante.
        /// </summary>
        /// <returns></returns>
        [HttpGet("all_products")]
        [ProducesResponseType(200, Type = typeof(List<GetProductDto>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllProductAsync()
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productService.GetAllProductAsync();
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
        /// Agrega una nueva calificación de un usuario acerca de un producto.
        /// </summary>
        /// <param name="addCalification"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(200,Type = typeof(bool))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAddClasificationProductAsync([FromBody] AddCalificationProductDto addCalification)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._productService.AddCalificationForProductAsync(addCalification);
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
