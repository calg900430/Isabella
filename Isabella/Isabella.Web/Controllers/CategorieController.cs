namespace Isabella.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;

    using Isabella.Web.ServicesControllers;
    using Isabella.Web.ViewModels.ProductViewModel;
    using Isabella.Common.Dtos.Categorie;
    using Isabella.Web.ViewModels.CategorieViewModel;

    /// <summary>
    /// Controlador de Productos 
    /// </summary>
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] //Omite este controlador de la documentación API
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
        /// Obtiene todas las categorias.
        /// </summary>
        /// <returns></returns>
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Index()
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
                    {
                        var GetAllCategorie = result.Data.Select(c => new GetCategorieViewModel
                        {
                          Id = c.Id,
                          Name = c.Name
                        }).ToList();
                       return View(GetAllCategorie); 
                    }
                    else
                    return View(result.Message);
                }
                else
                return View(); 
            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary>
        /// <param name="addCategory"></param>
        /// <returns></returns>
        [HttpPost("add/category")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddCategoryAsync(AddCategorieDto addCategory)
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
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryDto updateCategory)
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
