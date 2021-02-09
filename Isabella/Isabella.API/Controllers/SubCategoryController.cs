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
    using Common.Extras;
    using Common.RepositorysDtos;
    using Common.Dtos.SubCategory;
    using Extras;
    using Models;
    using RepositorysModels;
    using ServicesControllers;
   
    /// <summary>
    /// Controlador para las subcategorias.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoryController : Controller
    {
        private readonly SubCategoryServiceController _subCategoryService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="subCategoryServiceController"></param>
        public SubCategoryController(SubCategoryServiceController subCategoryServiceController)
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
        public async Task<IActionResult> AddCategoryStandardAsync([FromBody] AddSubCategoryDto addSubCategory)
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
        public async Task<IActionResult> GetCategoryForIdAsync(int Id)
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
        public async Task<IActionResult> GetCategoryForNameAsync(string Name)
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
        /// Obtiene todas las subcategorias disponibles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_subcategory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllCategoriesAsync()
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
    }
}
