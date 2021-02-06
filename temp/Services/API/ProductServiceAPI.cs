namespace Isabella.Web.Services.API
{
    using System.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using AutoMapper;

    using Common.Dtos.Product;
    using Common;
    using Common.Extras;
    using Models.Entities;
    using Data;
    using Repositorys.API;
   
    /// <summary>
    /// Servicio para gestionar con la entidad productos.
    /// </summary>
    public class ProductServiceAPI : GenericService<Product>, IProductRepositoryAPI
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="mapper"></param>
        public ProductServiceAPI(DataContext dataContext, IMapper mapper) : base(dataContext)
        {
            this._dataContext = dataContext;
            this._mapper = mapper;
        }

        /// <summary>
        /// Accede a todos los productos del restaurante.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductDto>>> GetAllProductAsync()
        {
            ServiceResponse<List<GetProductDto>> serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                //Accede a todos los productos del restaurante
                var list_products = await _dataContext.Products
                .Include(c => c.ProductImages)
                .Include(c => c.Category)
                .ToListAsync();
                if (list_products == null || list_products.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El restaurante no tiene productos en este momento.";
                    return serviceResponse;
                }
                else
                {
                   //Mapea de Producto a GetProductDto para enviar la información en el Service Response
                   serviceResponse.Data = list_products.Select(c => new GetProductDto 
                   {
                       Average = c.Average,
                       CategorieId = c.Category.Id,
                       Description = c.Description,
                       Id = c.Id,
                       IsAvailabe = c.IsAvailabe,
                       Name = c.Name,
                       Price = c.Price,
                       Stock = c.Stock,
                       GetProductImage = c.ProductImages.Select(x => new GetProductImageDto
                       {
                           Id = x.Id,
                           ImageFullPath = x.ImageFullPath,
                           ImageProductPath = x.ImageProductPath

                       }).ToList()
                   }).ToList();
                   serviceResponse.Message = "Se obtuvieron todos los productos disponibles del restaurante.";
                   serviceResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Accede a un producto del restaurante por su Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetProductDto>> GetProductByIdAsync(int id)
        {
            ServiceResponse<GetProductDto> serviceResponse = new ServiceResponse<GetProductDto>();
            try
            {
                //Accede a un Producto de la base de datos según el id.
                Product product = await _dataContext.Products
                .Include(c => c.Category)
                .Include(c => c.ProductImages)
                .FirstOrDefaultAsync(u => u.Id == id);
                if (product == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No se encuentra el producto en la base de datos.";
                    return serviceResponse;
                }
                else
                {
                    //Mapea de Producto a GetProductDto para enviar la información en el Service Response
                    serviceResponse.Data = new GetProductDto
                    {
                        Average = product.Average,
                        Category = product.Category.Name,
                        Description = product.Description,
                        Id = product.Id,
                        IsAvailabe = product.IsAvailabe,
                        Name = product.Name,
                        Price = product.Price,
                        Stock = product.Stock,
                        GetProductImage = product.ProductImages.Select(c => new GetProductImageDto
                        {
                            Id = c.Id,
                            ImageFullPath = c.ImageFullPath,
                            ImageProductPath = c.ImageProductPath
                            
                        }).ToList()
                    };
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Se obtuvo el producto de la base de datos.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Agrega una calificación a un producto del restaurante.
        /// </summary>
        /// <param name="addcalificationProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddCalificationForProductAsync(AddCalificationProductDto addcalificationProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if(addcalificationProduct == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Debe enviar los datos necesarios para agregar una calificación de un producto.";
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users
               .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(addcalificationProduct.CodeUser))
               .ConfigureAwait(false);
                if(user == null)
                {
                   serviceResponse.Data = false;
                   serviceResponse.Success = false;
                   serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                   return serviceResponse;
                }
                //Obtiene todos los productos del restaurante
                var all_restaurant = await this._dataContext.Products.ToListAsync();
                if (all_restaurant == null || all_restaurant.Count() == 0)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El restaurante no tiene productos en este momento.";
                    return serviceResponse;
                }
                //Verifica que el producto se encuentre en el restaurante
                var product = all_restaurant.FirstOrDefault(c => c.Id == addcalificationProduct.ProductId);
                if (product == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El producto no se encuentra en la base de datos.";
                    return serviceResponse;
                }
                //Mapea de AddCalificationProductDto a CalificationProduct
                var calificationproduct = this._mapper.Map<CalificationProduct>(addcalificationProduct);
                //Asigna los campos restantes
                calificationproduct.Id = product.Id;
                calificationproduct.DateCreated = DateTime.UtcNow;
                calificationproduct.User = user;
                calificationproduct.Product = product;
                //Agrega la calificación del producto a la base de datos
                await this._dataContext.CalificationProducts.AddAsync(calificationproduct);
                //Guarda los cambios en la base de datos.
                await this._dataContext.SaveChangesAsync();
                //Obtiene todas las calificaciones actuales del producto
                var all_calification = await this._dataContext.CalificationProducts.ToListAsync();
                //Calcula el promedio de las calificaciones
                product.Average = (float) all_calification.Average(c => c.Calification);
                //Actualiza el producto.
                await UpdateAsync(product);
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = "Se ha agregado su calificación acerca del producto.";
            }
            catch (SystemException)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Agrega un nuevo producto de categoria especial(Pizza o Pastas).
        /// </summary>
        /// <param name="addProductWithAdd"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductWithAddAsync(AddProductWithAddDto addProductWithAdd)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if(addProductWithAdd == null)
                {
                   serviceResponse.Data = false;
                   serviceResponse.Success = false;
                   serviceResponse.Message = "Debe enviar los datos necesarios para agregar el producto..";
                   return serviceResponse;
                }
                //Verifica que el usuario admin este registrado en la base de datos
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(addProductWithAdd.CodeUser))
                .ConfigureAwait(false);
                if(user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No se encuentra el usuario admin en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto a agregar sea combinado
                if(addProductWithAdd.Categorie != EnumCategorieForProductWithAdd.Pastas 
                || addProductWithAdd.Categorie != EnumCategorieForProductWithAdd.Pizzas)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "La categoria del producto no es válida para un producto con agregados.";
                    return serviceResponse;
                }
                //Mapea de un producto agregado a producto
                var product = this._mapper.Map<Product>(addProductWithAdd);
                //Verifica si el producto tiene imagenes
                if (addProductWithAdd.ImagesProduct != null)
                {
                    //Ruta de la carpeta donde las imagenes de las pizzas
                    string path_folder = "";
                    if (addProductWithAdd.Categorie == EnumCategorieForProductWithAdd.Pizzas)
                    path_folder = "wwwroot//images//images_products//pizzas";
                    else
                    path_folder = "wwwroot//images//images_products//pastas";
                    //Nombre para la imagen
                    string filename;
                    //Lista para almacenar las URLs de las imagenes de las publicaciones
                    List<ImageProduct> Images = new List<ImageProduct>();
                    //Almacena las imagenes de las publicaciones
                    foreach (byte[] image in addProductWithAdd.ImagesProduct)
                    {
                        //Genera un nombre para la imagen
                        filename = Guid.NewGuid().ToString();
                        //Guarda la imagen en el servidor
                        using (var memory = new MemoryStream(image))
                        {
                            if (UploadFileServiceCommon.UploadFile(memory, path_folder, filename))
                            {
                                //Genera la ruta de la imagen
                                var path_file = $"~/images/image_publications/{filename}";
                                //Guarda la relación 
                                ImageProduct images_Publication = new ImageProduct
                                {
                                   ImageProductPath = path_file,
                                   Product = product,
                                };
                                Images.Add(images_Publication);
                            }
                        }
                    } 
                }
                //Guarda el producto en la base de datos.
                await this.CreateAsync(product).ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Message = "Se ha agregado el producto a la base de datos.";
                return serviceResponse;
            }
            catch(SystemException)
            {
               serviceResponse.Data = false;
               serviceResponse.Success = false;
               serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
               return serviceResponse; 
            }
            catch(Exception ex)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega un nuevo producto
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductAsync(AddProductDto addProduct)
        {
           ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if(addProduct == null)
                {
                   serviceResponse.Data = false;
                   serviceResponse.Success = false;
                   serviceResponse.Message = "Debe enviar los datos necesarios para agregar el producto..";
                   return serviceResponse;
                }
                //Verifica que el usuario admin este registrado en la base de datos
                var user = await this._dataContext.Users
                .FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(addProduct.CodeUser))
                .ConfigureAwait(false);
                if(user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No se encuentra el usuario admin en la base de datos.";
                    return serviceResponse;
                }
                //Verifica que el producto a agregar sea combinado
                if(addProduct.Categorie != EnumCategories.Bebidas || 
                addProduct.Categorie != EnumCategories.Entrantes 
                || addProduct.Categorie != EnumCategories.Mariscos || 
                addProduct.Categorie != EnumCategories.PlatosPrincipales || 
                addProduct.Categorie != EnumCategories.Postres || addProduct.Categorie != EnumCategories.VinosLicores)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "La categoria del producto no es válida para un producto sin agregados.";
                    return serviceResponse;
                }
                //Mapea de un producto agregado a producto
                var product = this._mapper.Map<Product>(addProduct);
                //Verifica si el producto tiene imagenes
                if (addProduct.ImagesProduct != null)
                {
                    //Ruta de la carpeta donde las imagenes de las pizzas
                    string path_folder = "";
                    if (addProduct.Categorie == EnumCategories.Entrantes)
                    path_folder = "wwwroot//images//images_products//entrantes";
                    else if(addProduct.Categorie == EnumCategories.Bebidas)
                    path_folder = "wwwroot//images//images_products//bebidas";
                    else if(addProduct.Categorie == EnumCategories.PlatosPrincipales)
                    path_folder = "wwwroot//images//images_products//paltos_principales";
                    else if (addProduct.Categorie == EnumCategories.Postres)
                    path_folder = "wwwroot//images//images_products//postres";
                    else
                    path_folder = "wwwroot//images//images_products//vinos_licores"; 
                    //Nombre para la imagen
                    string filename;
                    //Lista para almacenar las URLs de las imagenes de las publicaciones
                    List<ImageProduct> Images = new List<ImageProduct>();
                    //Almacena las imagenes de las publicaciones
                    foreach (byte[] image in addProduct.ImagesProduct)
                    {
                        //Genera un nombre para la imagen
                        filename = Guid.NewGuid().ToString();
                        //Guarda la imagen en el servidor
                        using (var memory = new MemoryStream(image))
                        {
                            if (UploadFileServiceCommon.UploadFile(memory, path_folder, filename))
                            {
                                //Genera la ruta de la imagen
                                var path_file = $"~/images/image_publications/{filename}";
                                //Guarda la relación 
                                ImageProduct images_Publication = new ImageProduct
                                {
                                   ImageProductPath = path_file,
                                   Product = product,
                                };
                                Images.Add(images_Publication);
                            }
                        }
                    } 
                }
                //Guarda el producto en la base de datos.
                await this.CreateAsync(product).ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Message = "Se ha agregado el producto a la base de datos.";
                return serviceResponse;
            }
            catch(SystemException)
            {
               serviceResponse.Data = false;
               serviceResponse.Success = false;
               serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
               return serviceResponse; 
            }
            catch(Exception ex)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }
        }

    }
}
