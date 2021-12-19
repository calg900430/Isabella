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
    using Common.RepositorysDtos;
    using Common.Dtos.SubCategorie;
    using ServicesControllers;
   
    /// <summary>
    /// Controlador para las subcategorias.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SubCategorieController : Controller
    {
        private readonly SubCategorieServiceController _subCategoryService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="subCategoryServiceController"></param>
        public SubCategorieController(SubCategorieServiceController subCategoryServiceController)
        {
            this._subCategoryService = subCategoryServiceController;
        }

        /// <summary>
        /// Agrega una nueva subcategoria.
        /// </summary>
        /// <param name="addSubCategory"></param>
        /// <returns></returns>
        [HttpPost("add/subcategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddSubCategoryStandardAsync([FromBody] AddSubCategorieToProductDto addSubCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService.AddSubCategoryAsync(addSubCategory)
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
        /// Obtiene una subcategoria por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("get/subcategory_id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSubCategoryForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService.GetSubCategoryForIdAsync(Id)
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
        /// Obtiene una subcategoria por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpGet("get/subcategory_name")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSubCategoryForNameAsync(string Name)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService
                    .GetSubCategoryForNameAsync(Name)
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
        /// Obtiene todas las subcategorias.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_subcategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllSubCategoriesAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService
                    .GetAllSubCategoryAsync()
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
        /// Obtiene todas las subcategorias disponibles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_subcategory_available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllSubCategoriesIsAvailableAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService
                    .GetAllSubCategoryIsAvailableAsync()
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
        /// Obtiene todas las subcategorias no disponibles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_subcategory_notavailable")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllSubCategoriesIsNotAvailableAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService
                    .GetAllSubCategoryIsNotAvailableAsync()
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
        /// Elimina una subcategoria.
        /// </summary>
        /// <param name="SubCategoryId"></param>
        /// <returns></returns>
        [HttpDelete("del/subcategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteSubCategoryAsync(int SubCategoryId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService
                    .DeleteSubCategoryAsync(SubCategoryId)
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
        /// Actualiza una subcategoria.
        /// </summary>
        /// <param name="updateSubCategory"></param>
        /// <returns></returns>
        [HttpPut("update/subcategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateSubCategoryAsync([FromBody] UpdateSubCategorieDto updateSubCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._subCategoryService
                    .UpdateSubCategoryAsync(updateSubCategory)
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
