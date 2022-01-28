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
        /// Muestra la View para crear una categoria.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public IActionResult Create()
        {
            try
            {
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
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AddCategorieDto addCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this
                    ._categoryService.AddCategoryAsync(addCategory)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return RedirectToAction(nameof(Index)); 
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
        /// Muestra la View para actualizar una categoria.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Edit")]
        public IActionResult Edit()
        {
            try
            {
                return View();
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
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(UpdateCategorieDto updateCategory)
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
                    return RedirectToAction(nameof(Index));
                    else
                    return BadRequest(result.Message);
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
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int CategoryId)
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
                    return RedirectToAction(nameof(Index));
                    else
                    return BadRequest(result.Message);
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
