namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;

    using Common;
    using Common.Dtos.ProductAggregate;
    using Common.RepositorysDtos;
    using Common.Extras;
    using RepositorysModels;
    using Models;
    using Data;
    using Extras;

    /// <summary>
    /// 
    /// </summary>
    public class ProductAggregateServiceController : IProductAggregateRepositoryDto
    {
        private readonly IProductAggregateRepositoryModel _productAggregateRepositoryModel;
        private readonly ICategoryRepositoryModel _categorRepositoryModel; 

        /// <summary>
        /// Constructor
        /// </summary>
        public ProductAggregateServiceController(IProductAggregateRepositoryModel productAggregateRepositoryModel,
        ICategoryRepositoryModel categoryRepositoryModel)
        {
            this._productAggregateRepositoryModel = productAggregateRepositoryModel;
            this._categorRepositoryModel = categoryRepositoryModel;
        }

        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="addProductStandard"></param> 
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductAggregateAsync(AddProductAggregateDto addProductStandard)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addProductStandard == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica que la categoria es valida
                var category = await this._categorRepositoryModel
                .GetCategoryForIdAsync(addProductStandard.CategoryId)
                .ConfigureAwait(false);
                if (category == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeCategory_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCategory_NotFound);
                    return serviceResponse;
                }
                //Mapea de AddProductStandardDto a ProductStandard
                var new_product = new ProductAggregate
                {
                    Category = category,
                    DateCreated = DateTime.UtcNow,
                    DateUpdate = DateTime.UtcNow,
                    Description = addProductStandard.Description,
                    IsAvailabe = addProductStandard.IsAvailabe,
                    Name = addProductStandard.Name,
                    Price = addProductStandard.Price,
                    Stock = addProductStandard.Stock,
                    LastBuy = DateTime.UtcNow,
                };
                await this._productAggregateRepositoryModel
                .AddProductAggregateAsync(new_product)
                .ConfigureAwait(false);
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega una imagen de un producto(Usando IFormFile).
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddImageProductAggregateAsync(IFormFile formFile, int ProductId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que la imagen, cumpla con los reguerimientos para poderla almacenar en la base de datos.
                if (formFile == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                if (formFile.Length <= 0)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                if (formFile.Length > Constants.MAX_LENTHG_IMAGE_PROFILE_USER)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageUserNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageUserNotValide);
                    return serviceResponse;
                }
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Agrega las imagenes
                var add_images = await this._productAggregateRepositoryModel
                .AddImageForProductAggregateAsync(formFile, product)
                .ConfigureAwait(false);
                if (!add_images)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageErrorCreated;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageErrorCreated);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega una imagen a un producto.
        /// </summary>
        /// <param name="addImageProductStandard"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddImageProductAggregateAsync(AddImageProductAggregateDto addImageProductStandard)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica que la imagen, cumpla con los reguerimientos para poderla almacenar en la base de datos.
                if (addImageProductStandard == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica que la imagen, cumpla con los reguerimientos para poderla almacenar en la base de datos.
                if (addImageProductStandard.Image == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                if (addImageProductStandard.Image.Length <= 0)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                if (addImageProductStandard.Image.Length > Constants.MAX_LENTHG_IMAGE_PRODUCT)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageUserNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageUserNotValide);
                    return serviceResponse;
                }
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(addImageProductStandard.ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Agrega las imagenes
                var add_images = await this._productAggregateRepositoryModel
                .AddImageForProductAggregateAsync(addImageProductStandard.Image, product)
                .ConfigureAwait(false);
                if (!add_images)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageErrorCreated;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageErrorCreated);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Borra una imagen de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        public async Task<ServiceResponse<bool>> DeleteImageProductAggregateAsync(int ProductId, int ImageId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene la imagen especifica.
                var image_product_standard = await this._productAggregateRepositoryModel
                .GetImageProductAggregateAsync(product, ImageId)
                .ConfigureAwait(false);
                if (image_product_standard == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageNotExist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageNotExist);
                    return serviceResponse;
                }
                //Elimina la imagen especifica.
                await this._productAggregateRepositoryModel
                .DeleteImageProductAggregateAsync(image_product_standard).ConfigureAwait(false);
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Pone un producto en no disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DisableProductAggregateAsync(int ProductId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                product.IsAvailabe = false;
                var enable_product = await this._productAggregateRepositoryModel
                .UpdateProductAggregateAsync(product)
                .ConfigureAwait(false);
                if (enable_product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Pone un producto en disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> EnableProductAggregateAsync(int ProductId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                product.IsAvailabe = true;
                var enable_product = await this._productAggregateRepositoryModel
                .UpdateProductAggregateAsync(product)
                .ConfigureAwait(false);
                if (enable_product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todas las imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetImageProductAggregateDto>>> GetAllImageProductAggregateAsync(int ProductId)
        {
            ServiceResponse<List<GetImageProductAggregateDto>> serviceResponse = new ServiceResponse<List<GetImageProductAggregateDto>>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Obtiene todas las imagenes del producto.
                var all_images = await this._productAggregateRepositoryModel
                .GetAllImageProductAggregateAsync(product)
                .ConfigureAwait(false);
                if (all_images == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ProductNotImage;
                    serviceResponse.Data = null;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ProductNotImage);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = all_images.Select(c => new GetImageProductAggregateDto
                {
                    Image = c.Image,
                    ImageId = c.Id,
                    ProductId = c.ProductAggregate.Id
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene todos los productos disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductAggregateDto>>> GetAllProductAggregateAsync()
        {
            ServiceResponse<List<GetProductAggregateDto>> serviceResponse = new ServiceResponse<List<GetProductAggregateDto>>();
            try
            {
                //Obtiene los productos disponibles
                var all_productstandard = await this._productAggregateRepositoryModel
                .GetAllProductAggregateWithCategoryAsync()
                .ConfigureAwait(false);
                if (all_productstandard == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_AllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_AllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = all_productstandard.Select(c => new GetProductAggregateDto
                {
                    Id = c.Id,
                    Category = new Common.Dtos.Category.GetCategoryDto
                    {
                        Id = c.Category.Id,
                        Name = c.Category.Name,
                    },
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                }).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una cantidad especifica de imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <param name="CantImages"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetImageProductAggregateDto>>> GetCantImageProductAggregateAsync(int ProductId, int ImageId, int CantImages)
        {
            ServiceResponse<List<GetImageProductAggregateDto>> serviceResponse = new ServiceResponse<List<GetImageProductAggregateDto>>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Obtiene todas las imagenes disponibles del producto.
                var all_images_product = await this._productAggregateRepositoryModel
                .GetAllImageProductAggregateAsync(product)
                .ConfigureAwait(false);
                if (all_images_product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ProductNotImage;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ProductNotImage);
                    return serviceResponse;
                }
                //Obtiene la imagen de referencia
                var image_reference = all_images_product.FirstOrDefault(c => c.Id == ImageId);
                if (image_reference == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageNotExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageNotExist);
                    return serviceResponse;
                }
                //Obtiene el index de la imagen de referencia.
                var index_image_reference = all_images_product.LastIndexOf(image_reference);
                //Obtiene el index de la ultima imagen que se agrego del producto.
                var index_lastimage_reference = all_images_product.LastIndexOf(all_images_product.Last());
                //No hay nuevas imagenes disponibles
                if (index_image_reference == index_lastimage_reference)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ProductNotNewImage;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ProductNotNewImage);
                    return serviceResponse;
                }
                //Obtiene la cantidad de imagenes disponibles a partir de la imagen de referencia.
                var image_available = index_lastimage_reference - index_image_reference;
                //Obtiene el Index siguiente al de referencia
                var index_next_reference = all_images_product.LastIndexOf(all_images_product[index_image_reference + 1]);
                //Envia la cantidad de imagenesdisponibles.
                if (CantImages >= image_available)
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_images_to_send = all_images_product.GetRange(index_next_reference, image_available);
                    serviceResponse.Data = list_images_to_send.Select(c => new GetImageProductAggregateDto
                    {
                        Image = c.Image,
                        ImageId = c.Id,
                        ProductId = product.Id
                    }).ToList();
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
                //Envia las productos solicitadas por el usuario.
                else
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_images_to_send = all_images_product.GetRange(index_next_reference, CantImages);
                    serviceResponse.Data = list_images_to_send.Select(c => new GetImageProductAggregateDto
                    {
                        Image = c.Image,
                        ImageId = c.Id,
                        ProductId = product.Id
                    }).ToList();
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una cantidad determinada de productos dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="CantProduct"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetProductAggregateDto>>> GetCantProductAggregateAsync(int ProductId, int CantProduct)
        {
            ServiceResponse<List<GetProductAggregateDto>> serviceResponse = new ServiceResponse<List<GetProductAggregateDto>>();
            try
            {
                if (CantProduct < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeUser_ValueNotValide;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeUser_ValueNotValide);
                    return serviceResponse;
                }
                //Obtiene todos los productos standards del sistema.
                var all_productsstandard = await this._productAggregateRepositoryModel
                .GetAllProductAggregateWithCategoryAsync()
                .ConfigureAwait(false);
                if (all_productsstandard == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_AllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_AllNotFound);
                    return serviceResponse;
                }
                //Obtiene el producto standard de referencia.
                var productstandard_reference = all_productsstandard
                .Where(c => c.Id == ProductId)
                .FirstOrDefault();
                if (productstandard_reference == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Verifica si no hay nuevos productos standards.
                var begin_found_productstandard = all_productsstandard
                .FirstOrDefault(c => c.Id == productstandard_reference.Id + 1);
                if (begin_found_productstandard == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotNew;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotNew);
                    return serviceResponse;
                }
                //Obtiene el index del producto por el cual se debe comenzar a buscar los demas.
                var index_product = all_productsstandard.LastIndexOf(begin_found_productstandard);
                //Obtiene el Index del ultimo producto.
                var last_index = all_productsstandard.LastIndexOf(all_productsstandard.LastOrDefault());
                //Obtiene la cantidad de productos disponibles a partir del producto de referencia.
                var products_available = last_index - index_product + 1;
                //Envia la cantidad de productos disponibles.
                if (CantProduct >= products_available)
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_products_to_send = all_productsstandard.GetRange(index_product, products_available);
                    serviceResponse.Data = list_products_to_send.Select(c => new GetProductAggregateDto
                    {
                        Id = c.Id,
                        Category = new Common.Dtos.Category.GetCategoryDto
                        {
                            Id = c.Category.Id,
                            Name = c.Category.Name,
                        },
                        Description = c.Description,
                        Name = c.Name,
                        Price = c.Price,
                    }).ToList();
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
                //Envia las productos solicitadas por el usuario.
                else
                {
                    //Toma una cantidad de elementos contiguos a partir de un index de referencia.
                    var list_products_to_send = all_productsstandard.GetRange(index_product, CantProduct);
                    serviceResponse.Data = list_products_to_send.Select(c => new GetProductAggregateDto
                    {
                        Id = c.Id,
                        Category = new Common.Dtos.Category.GetCategoryDto
                        {
                            Id = c.Category.Id,
                            Name = c.Category.Name,
                        },
                        Description = c.Description,
                        Name = c.Name,
                        Price = c.Price,
                    }).ToList();
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene el Id del último producto.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<int>> GetIdOfLastProductAggregateAsync()
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();
            try
            {
                var last_id_product = await this._productAggregateRepositoryModel
                .GetIdOfLastProductAggregateAsync()
                .ConfigureAwait(false);
                if (last_id_product == -1)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_AllNotFound;
                    serviceResponse.Data = -1;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_AllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = last_id_product;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = -1;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una imagen determinada de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetImageProductAggregateDto>> GetImageProductAggregateForIdAsync(int ProductId, int ImageId)
        {
            ServiceResponse<GetImageProductAggregateDto> serviceResponse = new ServiceResponse<GetImageProductAggregateDto>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene la imagen especifica.
                var image_product = await this._productAggregateRepositoryModel
                .GetImageProductAggregateAsync(product, ImageId)
                .ConfigureAwait(false);
                if (image_product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ImageNotExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ImageNotExist);
                    return serviceResponse;
                }
                //Elimina la imagen especifica.
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetImageProductAggregateDto
                {
                    Image = image_product.Image,
                    ProductId = image_product.ProductAggregate.Id,
                    ImageId = image_product.Id,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Devuelve una lista con los Id de todas las imagenes del producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<int>>> GetListIdOfImageProductAggregateAsync(int ProductId)
        {
            ServiceResponse<List<int>> serviceResponse = new ServiceResponse<List<int>>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Obtiene todas las imagenes del producto.
                var all_images = await this._productAggregateRepositoryModel
                .GetAllImageProductAggregateAsync(product)
                .ConfigureAwait(false);
                if (all_images == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeImage_ProductNotImage;
                    serviceResponse.Data = null;
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeImage_ProductNotImage);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = all_images.Select(c => c.Id).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene un producto dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetProductAggregateDto>> GetProductAggregateForIdAsync(int Id)
        {
            ServiceResponse<GetProductAggregateDto> serviceResponse = new ServiceResponse<GetProductAggregateDto>();
            try
            {
                //Obtiene el producto.
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdWithCategoryAsync(Id)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetProductAggregateDto
                {
                    Id = product.Id,
                    Category = new Common.Dtos.Category.GetCategoryDto
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name,
                    },
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProductAggregate"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetProductAggregateDto>> UpdateProductAggregateAsync(UpdateProductAggregateDto updateProductAggregate)
        {
            ServiceResponse<GetProductAggregateDto> serviceResponse = new ServiceResponse<GetProductAggregateDto>();
            try
            {
                if (updateProductAggregate == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Obtiene el producto que se desea actualizar
                var product = await this._productAggregateRepositoryModel
                .GetProductAggregateForIdNotIncludeAsync(updateProductAggregate.ProductId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //Actualiza los campos del producto.
                if (updateProductAggregate.IsAvailabe != null)
                product.IsAvailabe = (bool)updateProductAggregate.IsAvailabe;
                if (updateProductAggregate.CategoryId != null)
                {
                    //Busca si la nueva categoria está en la base de datos.
                    var category = await this._categorRepositoryModel
                    .GetCategoryForIdAsync((int)updateProductAggregate.CategoryId)
                    .ConfigureAwait(false);
                    if (category == null)
                    {
                        serviceResponse.Code = CodeMessage.Code.CodeCategory_NotFound;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCategory_NotFound);
                        return serviceResponse;
                    }
                    product.Category = category;
                }
                if (updateProductAggregate.Description != null)
                product.Description = updateProductAggregate.Description;
                if (updateProductAggregate.Name != null)
                product.Name = updateProductAggregate.Name;
                if (updateProductAggregate.Price != null)
                product.Price = (decimal)updateProductAggregate.Price;
                if (updateProductAggregate.Stock != null)
                product.Stock = (int)updateProductAggregate.Stock;
                product.DateUpdate = DateTime.UtcNow;
                var update_product = await this._productAggregateRepositoryModel
                .UpdateProductAggregateAsync(product).
                ConfigureAwait(false);
                if (update_product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = new GetProductAggregateDto
                {
                    Description = product.Description,
                    Category = new Common.Dtos.Category.GetCategoryDto
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name,
                    },
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                };
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }
    }
}
