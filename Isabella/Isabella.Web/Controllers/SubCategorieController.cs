namespace Isabella.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Isabella.Web.ServicesControllers;
    using Isabella.Web.ViewModels.SubCategorieViewModel;
    using Microsoft.AspNetCore.Mvc;
    
    /// <summary>
    /// Controlador de Productos 
    /// </summary>
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] //Omite este controlador de la documentación API
    public class SubCategorieController: Controller
    {
        private readonly SubCategorieServiceController _subcategorieServiceController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subcategorieServiceController"></param>
        public SubCategorieController(SubCategorieServiceController subcategorieServiceController)
        {
            this._subcategorieServiceController = subcategorieServiceController;
        }


        /// <summary>
        /// Muestra todos las subcategorias disponibles en la base de datos
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            //Obtiene todos los productos con sus imagenes de la base de datos.
            try
            {
                var list_subcategorie = await this._subcategorieServiceController
                .GetAllSubCategorieWithProductAsync().ConfigureAwait(false);
                if (list_subcategorie.Success)
                {
                    var getAllImagesAggregateViewModel = list_subcategorie.Data
                    .Select(c => new GetSubCategorieViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Price = c.Price,
                        IsAvailable = c.IsAvailable,
                        GetProductViewModel = new ViewModels.ProductViewModel.GetProductViewModel
                        { 
                           Name = c.GetProductDto.Name,
                           Id = c.GetProductDto.Id,
                        }                        
                    }).ToList();
                    return View(getAllImagesAggregateViewModel);
                }
                else
                {
                    //TODO:Retorna página de que no existen los productos.
                    return BadRequest(list_subcategorie);
                }
            }
            catch (Exception ex)
            {
                //TODO:Retorna página de excepciones con el mensaje del motivo de la Excepcion.
                return BadRequest(ex.Message);
            }
        }
    }
}
