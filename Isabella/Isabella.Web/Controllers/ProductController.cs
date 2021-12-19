namespace Isabella.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using Isabella.Web.ServicesControllers;
    using Isabella.Web.ViewModels.ProductViewModel;
   

    /// <summary>
    /// Controlador de Productos 
    /// </summary>
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] //Omite este controlador de la documentación API
    public class ProductController : Controller
    {
        private readonly ProductServiceController _productServiceController;
        private readonly CategorieServiceController _categorieServiceController;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productServiceController"></param>
        /// <param name="categorieServiceController"></param>
        public ProductController(ProductServiceController productServiceController, 
        CategorieServiceController categorieServiceController)
        {
            this._productServiceController = productServiceController;
            this._categorieServiceController = categorieServiceController;
        }

        /// <summary>
        /// Muestra todos los productos disponibles en la base de datos
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            //Obtiene todos los productos con sus imagenes de la base de datos.
            try
            {
                var list_products = await this._productServiceController
                .GetProductsWithAllElement();
                if (list_products.Success)
                {
                    var getAllImagesProductViewModel = list_products.Data
                    .Select(c => new GetProductViewModel
                    {
                       Id = c.Id,
                       Name = c.Name,
                       GetAllImagesProducts = c.GetAllImagesProduct,
                       Price = c.Price,
                       IsAvailabe = c.IsAvailabe
                    }).ToList();
                    return View(getAllImagesProductViewModel);
                }
                else
                {
                    //TODO:Retorna página de que no existen los productos.
                    return View();
                }
            }
            catch
            {
                //TODO:Retorna página de excepciones con el mensaje del motivo de la Excepcion.
                return View();
            }
        }

        /// <summary>
        /// Muestra los detalles del producto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Details")]
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            //Obtiene el producto.
            var product = await this._productServiceController
            .GetProductForIdAsync(Id.Value)
            .ConfigureAwait(false);
            //Obtiene todas las imagenes del producto actual
            var all_images = await this._productServiceController
            .GetAllImageProductAsync(Id.Value)
            .ConfigureAwait(false);
            return View(new GetProductViewModel
            {
                SupportAggregate = product.Data.SupportAggregate,
                Id = product.Data.Id,
                Price = product.Data.Price,
                Name = product.Data.Name,
                IsAvailabe = product.Data.IsAvailabe,
                GetAllImagesProducts = all_images.Data,
                GetSubCategoryDtos = product.Data.GetSubCategoryDtos,
                Category = product.Data.Category,
                Description = product.Data.Description,   
            });
        }

        /// <summary>
        /// Muestra los elementos para crear un producto.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            //Obtiene todas las categorias
            var all_categorie = await this._categorieServiceController.GetAllCategoryAsync().ConfigureAwait(false);
            //Creamos una lista del tipo SelectListItem
            var selected_categories = new List<SelectListItem>();
            //Convierte la lista de categorias al tipo SelectListItem
            selected_categories = all_categorie.Data.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()

            }).ToList();
            //Asignamos la lista de categorias al ViewData
            //ViewData se pasa automaticamente a la vista.
            ViewBag.Categories = selected_categories;
            return View(); 
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel addProductViewModel, string Categories)
        {
            if(ModelState.IsValid)
            {
                //Parsea el Id de la categoria.
                var result = int.TryParse(Categories, out int IdCategorie);
                if (!result)
                {
                    return NotFound();
                }
                //Establece el valor de la categoria
                addProductViewModel.CategorieId = IdCategorie;
                //Guarda en la base de datos el nuevo objeto.
                var result_for_add_to_product = await this._productServiceController
                .AddProductAsync(addProductViewModel);
                if(result_for_add_to_product.Success)
                {
                    //Obtiene el Id del ultimo producto o sea el que acabamos de agregar
                    var DataId = await this._productServiceController.GetIdOfLastProductAsync().ConfigureAwait(false);
                    //Agrega la imagen del producto usando IFormFile
                    var is_save_imageawait = this._productServiceController.AddImageProductAsync(addProductViewModel.ImageFile, DataId.Data).ConfigureAwait(false);
                    //Retorna a la vista Index, donde debe aparecer el nuevo producto.
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                   return NotFound();
                }
            }
            else
            {
                //Retorna a la vista el mismo addProductViewModel para que verifique el error
                //y no tenga volver a llenar los datos.
                return View(addProductViewModel);
            }
        }


        /// <summary>
        /// Crea la view donde se va a actualizar el producto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var product = await this._productServiceController
            .GetProductForIdAsync(Id.Value).ConfigureAwait(false);
            if(product.Success)
            {
                //Obtiene todas las imagenes del producto.
                var all_image_product = await this._productServiceController
                .GetAllImageProductAsync(product.Data.Id).ConfigureAwait(false);
                ViewBag.AllImages = all_image_product.Data;
                //Obtiene todas las categorias
                var all_categorie = await this._categorieServiceController.GetAllCategoryAsync().ConfigureAwait(false);
                //Creamos una lista del tipo SelectListItem
                var selected_categories = new List<SelectListItem>();
                //Convierte la lista de categorias al tipo SelectListItem
                selected_categories = all_categorie.Data.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()

                }).ToList();
                //Asignamos la lista de categorias al ViewData
                //ViewData se pasa automaticamente a la vista.
                ViewBag.Categories = selected_categories;
                var updateproductviewmodel = new UpdateProductViewModel
                {
                    ProductId = product.Data.Id,
                };
                return View(updateproductviewmodel);
            }
            else
            {
               return NotFound();
            }   
        }

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProductViewModel"></param>
        /// <param name="Categories"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProductViewModel updateProductViewModel,string Categories)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   
                }
                catch
                {
                   
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        
        /// <summary>
        /// Crea la View donde se elimina el producto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(int? Id)
        {
            //Obtiene el producto
            try
            {
                if (Id == null)
                {
                    //Lanza la pagina de control de excepciones.
                    return NotFound();
                }
                var product = await this._productServiceController
                .GetProductForIdAsync(Id.Value).ConfigureAwait(false);
                if(!product.Success)
                {
                    return NotFound();
                }
                else
                {
                    var productviewmodel = new GetProductViewModel
                    {
                        Id = product.Data.Id,
                        Name = product.Data.Name,
                    };
                    return View(productviewmodel);
                }
            }
            catch
            {
                //Lanza la pagina de control de excepciones.
                return NotFound();
            }
        }

        /// <summary>
        /// Se elimina el producto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            //Obtiene el producto
            try
            {
                var result = await this._productServiceController
                .DeleteProductAsync(Id).ConfigureAwait(false);
                if(!result.Success)
                {
                    //Lanza la pagina de control de excepciones.
                    return NotFound();
                }
                else
                {
                   return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                //Lanza la pagina de control de excepciones.
                return NotFound();
            }
        }
    }
}
