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
    using Isabella.Common.Dtos.Categorie;
    using Isabella.Web.ViewModels.CategorieViewModel;
    using Microsoft.AspNetCore.Http;
    using Isabella.Common.Dtos.Product;

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
        /// Se elimina el producto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            //Obtiene el producto
            try
            {
                var result = await this._productServiceController
                .DeleteProductAsync(Id).ConfigureAwait(false);
                if (!result.Success)
                return NotFound(result);
                else
                return Ok(result);               
            }
            catch(Exception ex)
            {
                //Lanza la pagina de control de excepciones.
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Muestra los elementos para crear un producto.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                //Obtiene todas las categorias
                var all_categorie = await this._categorieServiceController.GetAllCategoryAsync()
                .ConfigureAwait(false);
                var categories = all_categorie.Data.Select(c => new GetCategorieViewModel
                {
                    Name = c.Name,
                    Id = c.Id

                }).ToList();
                //Asignamos la lista de categorias al ViewBag para acceder a el en la vista.
                ViewBag.Categories = categories;
                return View();
            }
            catch (Exception ex)
            {
                //Retorna la vista de manejo de Excepciones
                return View(ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="addProductViewModel"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] AddProductViewModel addProductViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Guarda en la base de datos el nuevo objeto.
                    var result_for_add_to_product = await this._productServiceController
                    .AddProductToImageAsync(addProductViewModel);
                    if (result_for_add_to_product.Success)
                    {
                      return Ok();
                    }
                    else
                    {
                       return NotFound(result_for_add_to_product);
                    }
                }
                else
                {
                    //Retorna a la vista el mismo addProductViewModel para que verifique el error
                    //y no tenga volver a llenar los datos.
                    return View(addProductViewModel);
                }
            }
            catch(Exception ex)
            {
               //Retorna la vista de manejo de Excepciones
               return View(ex.Message);
            }
            
        }

        /// <summary>
        /// Crea la view donde se va a actualizar el producto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                var product = await this._productServiceController
                .GetProductForIdAsync(Id).ConfigureAwait(false);
                if(product.Success)
                {
                   //Obtiene todas las categorias
                   var all_categorie = await this._categorieServiceController.GetAllCategoryAsync().ConfigureAwait(false);
                   var categories = all_categorie.Data.Select(c => new GetCategorieViewModel
                   {
                      Id = c.Id,
                      Name = c.Name
                   }).ToList();
                   //Asignamos la lista de categorias y imagenes al ViewBag
                   //ViewBag a se pasa automaticamente a la vista.
                   ViewBag.Categories = categories;
                   //Obtiene las imagenes del producto.
                   var images = await this._productServiceController 
                   .GetAllImageProductAsync(product.Data.Id)
                   .ConfigureAwait(false);
                   var updateproductviewmodel = new UpdateProductViewModel
                   {
                      ProductId = product.Data.Id,
                      CategorieId = product.Data.Categorie.Id,
                      Description = product.Data.Description,
                      IsAvailable = product.Data.IsAvailabe,
                      Name = product.Data.Name,
                      Price = product.Data.Price,
                      Stock = product.Data.Stock,
                      SupportAggregate = product.Data.SupportAggregate,
                      Images = images.Data
                   };
                   return View(updateproductviewmodel);
                }
                else
                {
                   return NotFound();
                }            
            }
            catch
            {
                //TODO:Retorna página de excepciones con el mensaje del motivo de la Excepcion.
                return View();
            }

        }

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProductViewModel"></param>
        /// <returns></returns>
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromForm] UpdateProductViewModel updateProductViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var updateproduct = await this._productServiceController
                    .UpdateProductViewModelAsync(updateProductViewModel)
                    .ConfigureAwait(false);
                    if (updateproduct.Success)
                    return Ok();
                    else
                    {
                       return NotFound(updateproduct);
                    }
                }
                else
                {
                   return BadRequest();
                }
            }
            catch
            {
              return View();
            }          
        }

        /// <summary>
        /// Muestra los detalles del producto.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Details")]
        public async Task<IActionResult> Details(int Id)
        {
            try
            {
                //Obtiene el producto.
                var product = await this._productServiceController
                .GetProductForIdAsync(Id)
                .ConfigureAwait(false);
                //Obtiene todas las imagenes del producto actual
                var all_images = await this._productServiceController
                .GetAllImageProductAsync(Id)
                .ConfigureAwait(false);
                return View(new GetProductViewModel
                {
                    SupportAggregate = product.Data.SupportAggregate,
                    Id = product.Data.Id,
                    Price = product.Data.Price,
                    Name = product.Data.Name,
                    IsAvailabe = product.Data.IsAvailabe,
                    GetAllImagesProducts = all_images.Data,
                    GetSubCategories = product.Data.GetSubCategories,
                    Categorie = product.Data.Categorie,
                    Description = product.Data.Description,
                });
            }
            catch(Exception ex)
            {
               //Muestra la pagina de excepciones
               return View();
            }           
        }     
    }
}
