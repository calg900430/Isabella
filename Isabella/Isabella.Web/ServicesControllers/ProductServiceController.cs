namespace Isabella.Web.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.AspNetCore.Http;

    using Common;
    using Common.Dtos.Product;
    using Common.RepositorysDtos;
    using Helpers;
    using Helpers.RepositoryHelpers;
    using Models.Entities;
    using Resources;
    //using Castle.Core.Logging;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Servicio para el controlador de los productos.
    /// </summary>
    public class ProductServiceController : IProductRepositoryDto
    {
        private readonly ServiceGenericHelper<Product> _serviceGenericProductHelper;
        private readonly ServiceGenericHelper<Category> _serviceGenericCategoryHelper;
        private readonly ServiceGenericHelper<ImageProduct> _serviceGenericImageProductHelper;
        private readonly ServiceGenericHelper<SubCategory> _serviceGenericSubCategoryHelper;
        private readonly ILogger<ProductServiceController> _logger;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProductGenericHelper"></param>
        /// <param name="serviceCategoryGenericHelper"></param>
        /// <param name="serviceGenericImageProductHelper"></param>
        /// <param name="serviceGenericSubCategoryHelper"></param>
        /// <param name="logger"></param>
        public ProductServiceController(ServiceGenericHelper<Product> serviceProductGenericHelper, 
        ServiceGenericHelper<Category> serviceCategoryGenericHelper, 
        ServiceGenericHelper<ImageProduct> serviceGenericImageProductHelper,
        ServiceGenericHelper<SubCategory> serviceGenericSubCategoryHelper,
        ILogger<ProductServiceController> logger)
        {
            this._serviceGenericProductHelper = serviceProductGenericHelper;
            this._serviceGenericCategoryHelper = serviceCategoryGenericHelper;
            this._serviceGenericImageProductHelper = serviceGenericImageProductHelper;
            this._serviceGenericSubCategoryHelper = serviceGenericSubCategoryHelper;
            this._logger = logger;
        }

        /// <summary>
        /// Agregar un nuevo producto.
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductAsync(AddProductDto addProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addProduct == null)
                {
                    serviceResponse.Code = (int) GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    _logger.LogWarning(1, GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
                    return serviceResponse;
                }
                if (addProduct.Stock < 1 || addProduct.Price < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    _logger.LogWarning(2, GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative));
                    return serviceResponse;
                }
                //Verifica que la categoria exista.
                var category = await this._serviceGenericCategoryHelper
                .GetLoadAsync(c => c.Id == addProduct.CategorieId)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                //Verifica que el nombre no este en uso
                var product_exist = await this._serviceGenericProductHelper
                .WhereSingleEntityAsync(c => c.Name == addProduct.Name)
                .ConfigureAwait(false);
                if (product_exist != null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductExist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductExist);
                    return serviceResponse;
                }
                //Mapea de AddProductStandardDto a ProductStandard
                var new_product = new Product
                {
                    Category = category,
                    DateCreated = DateTime.UtcNow,
                    DateUpdate = DateTime.UtcNow,
                    Description = addProduct.Description,
                    IsAvailabe = addProduct.IsAvailabe,
                    Name = addProduct.Name,
                    Price = addProduct.Price,
                    Stock = addProduct.Stock,
                    LastBuy = DateTime.UtcNow,
                    SupportAggregate = addProduct.SupportAggregate,
                };
                //Guarda el nuevo producto en la base de datos.
                await this._serviceGenericProductHelper
                .AddEntityAsync(new_product)
                .ConfigureAwait(false);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericProductHelper.SaveChangesBDAsync();
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetProductDto>> UpdateProductAsync(UpdateProductDto updateProduct)
        {
            ServiceResponse<GetProductDto> serviceResponse = new ServiceResponse<GetProductDto>();
            try
            {
                if (updateProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Obtiene el producto que se desea actualizar
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == updateProduct.ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                //Actualiza los campos del producto.
                if (updateProduct.IsAvailabe != null)
                product.IsAvailabe = (bool)updateProduct.IsAvailabe;
                if (updateProduct.CategoryId != null)
                {
                    //Busca si la nueva categoria está en la base de datos.
                    var category = await this._serviceGenericCategoryHelper
                    .GetLoadAsync(c => c.Id == updateProduct.CategoryId)
                    .ConfigureAwait(false);
                    if (category == null)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                        return serviceResponse;
                    }
                    product.Category = category;
                }
                if (updateProduct.Description != null)
                product.Description = updateProduct.Description;
                if (updateProduct.Name != null)
                product.Name = updateProduct.Name;
                if (updateProduct.Price != null)
                { 
                    product.Price = (decimal)updateProduct.Price;
                    if (updateProduct.Price < 1)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                        return serviceResponse;
                    }
                }
                if (updateProduct.Stock != null)
                {
                    if (updateProduct.Stock < 1)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                        return serviceResponse;
                    }
                    product.Stock = (int)updateProduct.Stock;
                }
                if(updateProduct.SupportAggregate != null)
                product.SupportAggregate = (bool) updateProduct.SupportAggregate;
                product.DateUpdate = DateTime.UtcNow;
                //Actualiza la entidad
                this._serviceGenericProductHelper
                .UpdateEntity(product);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericProductHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetProductDto
                { 
                   Average = product.Average,
                   Description = product.Description,
                   Category = new Common.Dtos.Categorie.GetCategorieDto
                   {
                       Id = product.Category.Id,
                       Name = product.Category.Name,
                   },
                   Id = product.Id,
                   Name = product.Name,
                   Price = product.Price
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un producto dado su Id.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetProductDto>> GetProductForIdAsync(int ProductId)
        {
            ServiceResponse<GetProductDto> serviceResponse = new ServiceResponse<GetProductDto>();
            try
            {
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId, c => c.Category, c => c.SubCategories)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetProductDto
                {
                    Id = product.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name,
                    },
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Average = product.Average,
                    IsAvailabe = product.IsAvailabe,
                    SupportAggregate = product.SupportAggregate,
                    GetSubCategoryDtos = product.SubCategories.Select(c => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                       Id = c.Id,
                       Name = c.Name,
                       Price = c.Price,
                       ProductId = product.Id,
                       IsAvailable = c.IsAvailable,
                    }).ToList(),
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los elementos de todos los productos
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetAllElementsOfProduct>>> GetProductsWithAllElement()
        {
            ServiceResponse<List<GetAllElementsOfProduct>> serviceResponse = new ServiceResponse<List<GetAllElementsOfProduct>>();
            try
            {
                //Obtiene los productos disponibles
                var all_product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Category, x => x.SubCategories, x => x.Images)
                .ConfigureAwait(false);
                if (all_product == null || all_product.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_product.Select(c => new GetAllElementsOfProduct
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
                    Average = c.Average,
                    GetAllImagesProduct = c.Images.Select(x => new GetImageProductDto {
                    Image = x.Image,
                    ImageId = x.Id,
                    ProductId = c.Id
                    }).ToList(),
                    SupportAggregate = c.SupportAggregate,
                    GetSubCategoryDtos = c.SubCategories.Select(x => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductId = c.Id,
                        IsAvailable = x.IsAvailable,
                    }).ToList(),
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los elementos de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAllElementsOfProduct>> GetProductWithAllElement(int ProductId)
        {
            ServiceResponse<GetAllElementsOfProduct> serviceResponse = new ServiceResponse<GetAllElementsOfProduct>();
            try
            {
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId, c => c.Category, c => c.SubCategories)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAllElementsOfProduct
                {
                    Id = product.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name,
                    },
                    GetAllImagesProduct = product.Images.Select(x => new GetImageProductDto
                    {
                        Image = x.Image,
                        ImageId = x.Id,
                        ProductId =product.Id
                    }).ToList(),
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Average = product.Average,
                    IsAvailabe = product.IsAvailabe,
                    SupportAggregate = product.SupportAggregate,
                    GetSubCategoryDtos = product.SubCategories.Select(c => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Price = c.Price,
                        ProductId = product.Id,
                        IsAvailable = c.IsAvailable,
                    }).ToList(),
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los productos disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductDto>>> GetAllProductAsync()
        {
            ServiceResponse<List<GetProductDto>> serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                //Obtiene los productos disponibles
                var all_product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Category, x => x.SubCategories)
                .ConfigureAwait(false);
                if (all_product == null || all_product.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_product.Select(c => new GetProductDto
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
                    Average = c.Average,
                    SupportAggregate = c.SupportAggregate,
                    GetSubCategoryDtos = c.SubCategories.Select(x => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductId = c.Id,
                        IsAvailable = x.IsAvailable,
                    }).ToList(),
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene el Id del último producto.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<int>> GetIdOfLastProductAsync()
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();
            try
            {
                var last_product = await this._serviceGenericProductHelper
                .LastEntityAsync()
                .ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = last_product.Id;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = -1;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una imagen determinada de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetImageProductDto>> GetImageProductForIdAsync(int ProductId, int ImageId)
        {
            ServiceResponse<GetImageProductDto> serviceResponse = new ServiceResponse<GetImageProductDto>();
            try
            {
                //Obtiene el producto.
                var image_product = await this._serviceGenericImageProductHelper
                .WhereFirstEntityAsync(c => c.Id == ImageId, c => c.Product)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if(image_product == null || image_product.Image == null || (image_product.Product.Id != ProductId))
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageNotExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageNotExist);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetImageProductDto
                {
                    Image = image_product.Image,
                    ProductId = image_product.Product.Id,
                    ImageId = image_product.Id,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega una imagen de un producto(Usando IFormFile).
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddImageProductAsync(IFormFile formFile, int ProductId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que la imagen, cumpla con los reguerimientos para poderla almacenar en la base de datos.
                if (formFile == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length > Constants.MAX_LENTHG_IMAGE_PRODUCT)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageProductNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageProductNotValide);
                    return serviceResponse;
                }
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                //Nombre de la imagen
                var file = $"{Guid.NewGuid()}.jpg";
                //Ruta temporal donde la guardaremos antes de enviarla a la base de datos.
                var path = Path.Combine(Directory.GetCurrentDirectory(), file);
                //Crea el archivo de la imagen que se encuentra en memoria RAM y lo guarda en la ruta seleccionada.
                using (var stream = new FileStream(path, FileMode.Create))
                {
                   await formFile.CopyToAsync(stream).ConfigureAwait(false);
                };
                var arraybyte_image = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
                //Crea la relacion de la imagen con el producto.
                var imagen_product = new ImageProduct
                {
                    Image = arraybyte_image,
                    Product = product
                };
                //Guarda la imagen del producto.
                await this._serviceGenericImageProductHelper
                .AddEntityAsync(imagen_product)
                .ConfigureAwait(false);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericImageProductHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                //Borra el archivo de imagen temporal
                File.Delete(path);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega una imagen a un producto.
        /// </summary>
        /// <param name="addImageProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddImageProductAsync(AddImageProductDto addImageProduct)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addImageProduct == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la imagen, cumpla con los reguerimientos para poderla almacenar en la base de datos.
                if (addImageProduct.Image == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (addImageProduct.Image.Length <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (addImageProduct.Image.Length > Constants.MAX_LENTHG_IMAGE_PRODUCT)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageProductNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageProductNotValide);
                    return serviceResponse;
                }
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == addImageProduct.ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                var image_product = new ImageProduct
                {
                    Image = addImageProduct.Image,
                    Product = product
                };
                await this._serviceGenericImageProductHelper
                .AddEntityAsync(image_product)
                .ConfigureAwait(false);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericImageProductHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Borra una imagen de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteImageProductAsync(int ProductId, int ImageId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Obtiene la imagen deseada con su producto relacionado.
                var product_image = await this._serviceGenericImageProductHelper
                .WhereFirstEntityAsync(c => c.Id == ImageId, c => c.Product)
                .ConfigureAwait(false);
                //Verifica si la imagen existe y si pertenece al producto.
                if(product_image == null || (product_image.Product.Id != ProductId ))
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageNotExist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageNotExist);
                    return serviceResponse;
                }
                //Elimina la imagen especifica.
                this._serviceGenericImageProductHelper.RemoveEntity(product_image);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericImageProductHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Establece un producto en disponible o no disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> EnableProductAsync(int ProductId, bool enable)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                if(enable)
                product.IsAvailabe = true;
                else
                product.IsAvailabe = false;
                //Actualiza el producto.
                this._serviceGenericProductHelper.UpdateEntity(product);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericProductHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una cantidad determinada de productos dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="cantProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductDto>>> GetCantProductAsync(int ProductId, int cantProduct)
        {
            ServiceResponse<List<GetProductDto>> serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                if (cantProduct < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                var all_cant_product = await this._serviceGenericProductHelper
                .GetLoadAsync(product, cantProduct, c => c.Category, c => c.SubCategories)
                .ConfigureAwait(false);
                if (all_cant_product == null || all_cant_product.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotNew;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotNew);
                    return serviceResponse;
                }
                serviceResponse.Data = all_cant_product.Select(c => new GetProductDto
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
                    Average = c.Average,
                    SupportAggregate = c.SupportAggregate,
                    GetSubCategoryDtos = c.SubCategories.Select(x => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductId = c.Id,
                        IsAvailable = x.IsAvailable,
                    }).ToList(),
                }).ToList();
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Devuelve una lista con los Id de todas las imagenes del producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<int>>> GetListIdOfImageProductAsync(int ProductId)
        {
            ServiceResponse<List<int>> serviceResponse = new ServiceResponse<List<int>>();
            try
            {
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId, c => c.Images)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene imagenes.
                if (product.Images == null || product.Images.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImagesNoExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImagesNoExist);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = product.Images.Select(c => c.Id).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todas las imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetImageProductDto>>> GetAllImageProductAsync(int ProductId)
        {
            ServiceResponse<List<GetImageProductDto>> serviceResponse = new ServiceResponse<List<GetImageProductDto>>();
            try
            {
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId, c => c.Images)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene imagenes.
                if (product.Images == null || product.Images.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImagesNoExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImagesNoExist);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = product.Images.Select(c => new GetImageProductDto
                { 
                   Image = c.Image,
                   ImageId = c.Id,
                   ProductId = c.Product.Id
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una cantidad especifica de imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <param name="cantImages"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetImageProductDto>>> GetCantImageProductAsync(int ProductId, int ImageId, int cantImages)
        {
            ServiceResponse<List<GetImageProductDto>> serviceResponse = new ServiceResponse<List<GetImageProductDto>>();
            try
            {
                if (cantImages < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Obtiene el producto y sus imagenes.
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId, c => c.Images)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene imagenes.
                if (product.Images == null || product.Images.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImagesNoExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImagesNoExist);
                    return serviceResponse;
                }
                //Solicita la cantidad de imagenes deseadas del producto.
                var list_images = this._serviceGenericImageProductHelper.GetLoadAsync(ImageId, product.Images, cantImages);
                if(list_images == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageNotExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageNotExist);
                    return serviceResponse;
                }
                if (list_images.Count <= 0)
                {
                    serviceResponse.Code = (int) GetValueResourceFile.KeyResource.ProductNotNewImage;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotNewImage);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = list_images.Select(c => new GetImageProductDto
                {
                    Image = c.Image,
                    ImageId = c.Id,
                    ProductId = c.Product.Id
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un producto dado su Id si el mismo está disponible(Obtiene las subcategorias que están disponibles).
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetProductDto>> GetProductIsAvailableForIdAsync(int ProductId)
        {
            ServiceResponse<GetProductDto> serviceResponse = new ServiceResponse<GetProductDto>();
            try
            {
                //Obtiene el producto.
                var product = await this._serviceGenericProductHelper
                .WhereFirstEntityAsync(c => c.Id == ProductId && c.IsAvailabe == true,
                c => c.Category, c => c.SubCategories)
                .ConfigureAwait(false);
                //Verifica si el producto está disponible
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotIsAvailable);
                    return serviceResponse;
                }
                //Verifica si tiene subcategorias y selecciona solamente las que están disponibles.
                if(product.SubCategories.Any())
                product.SubCategories = product.SubCategories.Where(c => c.IsAvailable == true).ToList();
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetProductDto
                {
                    Id = product.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name,
                    },
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Average = product.Average,
                    IsAvailabe = product.IsAvailabe,
                    SupportAggregate = product.SupportAggregate,
                    GetSubCategoryDtos = product.SubCategories.Select(c => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Price = c.Price,
                        ProductId = product.Id,
                        IsAvailable = c.IsAvailable,
                    }).ToList(),
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los productos que esten disponibles para la venta(Obtiene las subcategorias que están disponibles).
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductDto>>> GetAllProductIsAvailableAsync()
        {
            ServiceResponse<List<GetProductDto>> serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                //Obtiene los productos disponibles
                var all_product = await this._serviceGenericProductHelper
                .WhereListEntityAsync(c => c.IsAvailabe == true , c => c.Category, x => x.SubCategories)
                .ConfigureAwait(false);
                if (all_product == null || all_product.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductAllNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductAllNotIsAvailable);
                    return serviceResponse;
                }
                //Verifica si tiene subcategorias y selecciona solamente las que están disponibles.
                foreach(Product product in all_product)
                {
                   if (product.SubCategories.Any())
                   product.SubCategories = product.SubCategories.Where(c => c.IsAvailable == true).ToList();
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_product.Select(c => new GetProductDto
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
                    Average = c.Average,
                    SupportAggregate = c.SupportAggregate,
                    GetSubCategoryDtos = c.SubCategories.Select(x => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductId = c.Id,
                        IsAvailable = x.IsAvailable
                    }).ToList(),
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una cantidad determinada de productos disponibles dado un producto de referencia y la cantidad.
        /// (Obtiene las subcategorias que están disponibles).
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="cantProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductDto>>> GetCantProductIsAvailableAsync(int ProductId, int cantProduct)
        {
            ServiceResponse<List<GetProductDto>> serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                if (cantProduct < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Obtiene el producto.
                var product_referenc = await this._serviceGenericProductHelper
                .WhereFirstEntityAsync(c => c.Id == ProductId && c.IsAvailabe == true)
                .ConfigureAwait(false);
                if (product_referenc == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotIsAvailable);
                    return serviceResponse;
                }
                var all_cant_product = await this._serviceGenericProductHelper
                .GetLoadAsync(product_referenc, cantProduct, c => c.IsAvailabe == true, c => c.Category, c => c.SubCategories)
                .ConfigureAwait(false);
                if (all_cant_product == null || all_cant_product.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotNew;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotNew);
                    return serviceResponse;
                }
                //Verifica si tiene subcategorias y selecciona solamente las que están disponibles.
                foreach (Product product in all_cant_product)
                {
                   if (product.SubCategories.Any())
                   product.SubCategories = product.SubCategories.Where(c => c.IsAvailable == true).ToList();
                }
                serviceResponse.Data = all_cant_product.Select(c => new GetProductDto
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
                    Average = c.Average,
                    SupportAggregate = c.SupportAggregate,
                    GetSubCategoryDtos = c.SubCategories.Select(x => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductId = c.Id,
                        IsAvailable = x.IsAvailable,
                    }).ToList(),
                }).ToList();
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los productos de una categoria determinada.
        /// (Obtiene las subcategorias que están disponibles).
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductDto>>> GetAllProductOfCategory(int CategoryId)
        {
            ServiceResponse<List<GetProductDto>> serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereFirstEntityAsync(c => c.Id == CategoryId)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                //Obtiene los productos
                var all_product = await this._serviceGenericProductHelper
                .WhereListEntityAsync(c => c.Category == category, c => c.Category, x => x.SubCategories)
                .ConfigureAwait(false);
                if (all_product == null || all_product.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductsOfCategoryNotAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductsOfCategoryNotAvailable);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_product.Select(c => new GetProductDto
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
                    Average = c.Average,
                    SupportAggregate = c.SupportAggregate,
                    GetSubCategoryDtos = c.SubCategories.Select(x => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductId = c.Id,
                        IsAvailable = x.IsAvailable,
                    }).ToList(),
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los productos disponibles de una categoria determinada.
        /// (Obtiene las subcategorias que están disponibles).
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductDto>>> GetAllProductIsAvailableOfCategory(int CategoryId)
        {
            ServiceResponse<List<GetProductDto>> serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                //Verifica que la categoria es valida
                var category = await this._serviceGenericCategoryHelper
                .WhereFirstEntityAsync(c => c.Id == CategoryId)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CategoryNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CategoryNotFound);
                    return serviceResponse;
                }
                //Obtiene los productos
                var all_product = await this._serviceGenericProductHelper
                .WhereListEntityAsync(c => c.Category == category && c.IsAvailabe == true, c => c.Category, x => x.SubCategories)
                .ConfigureAwait(false);
                if (all_product == null || all_product.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductsOfCategoryNotAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductsOfCategoryNotAvailable);
                    return serviceResponse;
                }
                //Verifica si tiene subcategorias y selecciona solamente las que están disponibles.
                foreach (Product product in all_product)
                {
                   if (product.SubCategories.Any())
                   product.SubCategories = product.SubCategories.Where(c => c.IsAvailable == true).ToList();
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_product.Select(c => new GetProductDto
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Categorie.GetCategorieDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
                    Average = c.Average,
                    SupportAggregate = c.SupportAggregate,
                    GetSubCategoryDtos = c.SubCategories.Select(x => new Common.Dtos.SubCategorie.GetSubCategorieDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductId = c.Id,
                        IsAvailable = x.IsAvailable,
                    }).ToList(),
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Elimina un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteProductAsync(int ProductId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var product = await this._serviceGenericProductHelper
                .GetLoadAsync(c => c.Id == ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                //Elimina el producto
                this._serviceGenericProductHelper.RemoveEntity(product);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericProductHelper
                .SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ExceptionDeleteEntity;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.ExceptionDeleteEntity);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }
    }
}
