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
    using Common.RepositorysDtos;
    using ServicesControllers;

    /// <summary>
    /// Controlador para las categorias de productos standards.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategorieController : Controller
    {
        private readonly CategorieServiceController _categoryService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="categoryServiceController"></param>
        public CategorieController(CategorieServiceController categoryServiceController)
        {
            this._categoryService = categoryServiceController;
        }

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary>
        /// <param name="addCategory"></param>
        /// <returns></returns>
        [HttpPost("add/category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddCategoryStandardAsync([FromBody] AddCategorieDto addCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._categoryService.AddCategoryAsync(addCategory)
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
        /// Obtiene una categoria por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("get/id_category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCategoryForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._categoryService.GetCategoryForIdAsync(Id)
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
        /// Obtiene una categoria por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpGet("get/category/id_name")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCategoryForNameAsync(string Name)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._categoryService
                    .GetCategoryForNameAsync(Name)
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
        /// Obtiene todas las categorias.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._categoryService
                    .GetAllCategoryAsync()
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
        /// Obtiene todas las categorias disponibles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_category_availables")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllCategoriesIsAvailableAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._categoryService.GetAllCategoryIsProductIsAvailableAsync();
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
        /// Elimina una categoria.
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        [HttpDelete("del/category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteCategoryAsync(int CategoryId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._categoryService
                    .DeleteCategoryAsync(CategoryId)
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
        /// Actualiza una categoria.
        /// </summary>
        /// <param name="updateCategory"></param>
        /// <returns></returns>
        [HttpPut("update/category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategorieDto updateCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._categoryService
                    .UpdateCategoryAsync(updateCategory)
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
