namespace Isabella.Web.Controllers
{
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Isabella.Web.ServicesControllers;
    using Microsoft.AspNetCore.Mvc;
    using Isabella.Web.ViewModels.AggregateViewModel;

    /// <summary>
    /// Controlador de Productos 
    /// </summary>
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] //Omite este controlador de la documentación API
    public class AggregateController : Controller
    {
        private readonly AggregateServiceController _aggregateServiceController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aggregateServiceController"></param>
        public AggregateController(AggregateServiceController aggregateServiceController)
        {
            this._aggregateServiceController = aggregateServiceController;
        }

        /// <summary>
        /// Muestra todos los agregados disponibles en la base de datos
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            //Obtiene todos los productos con sus imagenes de la base de datos.
            try
            {
                var list_aggregate = await this._aggregateServiceController
                .GetAggregatesWithAllElement().ConfigureAwait(false);
                if (list_aggregate.Success)
                {
                    var getAllImagesAggregateViewModel = list_aggregate.Data
                    .Select(c => new GetAggregateViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        GetAllImagesAggregate = c.GetAllImagesAggregate,
                        Price = c.Price,
                        IsAvailabe = c.IsAvailabe
                    }).ToList();
                    return View(getAllImagesAggregateViewModel);
                }
                else
                {
                    //TODO:Retorna página de que no existen los productos.
                    return BadRequest(list_aggregate);
                }
            }
            catch(Exception ex)
            {
                //TODO:Retorna página de excepciones con el mensaje del motivo de la Excepcion.
                return BadRequest(ex.Message);
            }
        }
    }
}
