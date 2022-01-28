namespace Isabella.Web.ServicesControllers
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;

    using Common;
    using Common.Dtos.Aggregate;
    using Common.RepositorysDtos;
    using Models;
    using Data;
    using Models.Entities;
    using Resources;
    using Helpers;
    using Helpers.RepositoryHelpers;
    using System.IO;
   
    /// <summary>
    /// Servicio para el controlador de los agregados.
    /// </summary>
    public class AggregateServiceController : IAggregateRepositoryDto  //IAggregateRepositoryDto
    {

        private readonly ServiceGenericHelper<Aggregate> _serviceGenericAggregateHelper;
        private readonly ServiceGenericHelper<Categorie> _serviceGenericCategoryHelper;
        private readonly ServiceGenericHelper<ImageAggregate> _serviceGenericImageAggregateHelper;
        private readonly ServiceGenericHelper<CantAggregate> _serviceGenericCantAggregateHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceGenericAggregateHelper"></param>
        /// <param name="serviceGenericCategoryHelper"></param>
        /// <param name="serviceGenericImageAggregateHelper"></param>
        /// <param name="serviceGenericCantAggregateHelper"></param>
        public AggregateServiceController(ServiceGenericHelper<Aggregate> serviceGenericAggregateHelper,
        ServiceGenericHelper<Categorie> serviceGenericCategoryHelper, 
        ServiceGenericHelper<ImageAggregate> serviceGenericImageAggregateHelper,
        ServiceGenericHelper<CantAggregate> serviceGenericCantAggregateHelper)
        {
            this._serviceGenericAggregateHelper = serviceGenericAggregateHelper;
            this._serviceGenericCategoryHelper = serviceGenericCategoryHelper;
            this._serviceGenericImageAggregateHelper = serviceGenericImageAggregateHelper;
            this._serviceGenericCantAggregateHelper = serviceGenericCantAggregateHelper;
        }

        /// <summary>
        /// Agrega un nuevo agregado.
        /// </summary>
        /// <param name="addAggregate"></param> 
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddAggregateAsync(AddAggregateDto addAggregate)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addAggregate == null)
                {
                    serviceResponse.Code = (int) GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (addAggregate.Stock < 1 || addAggregate.Price < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica que el nombre no este en uso
                var aggregate_exist = await this._serviceGenericAggregateHelper
                .WhereSingleEntityAsync(c => c.Name == addAggregate.Name)
                .ConfigureAwait(false);
                if (aggregate_exist != null)
                {
                    serviceResponse.Code = (int) GetValueResourceFile.KeyResource.AggregateExist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateExist);
                    return serviceResponse;
                }
                //Mapea de AddProductStandardDto a ProductStandard
                var new_aggregate = new Aggregate
                {
                    DateCreated = DateTime.UtcNow,
                    DateUpdate = DateTime.UtcNow,
                    Description = addAggregate.Description,
                    IsAvailabe = addAggregate.IsAvailabe,
                    Name = addAggregate.Name,
                    Price = addAggregate.Price,
                    Stock = addAggregate.Stock,
                    LastBuy = DateTime.UtcNow,
                };
                await this._serviceGenericAggregateHelper
                .AddEntityAsync(new_aggregate)
                .ConfigureAwait(false);
                await this._serviceGenericAggregateHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
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

        /// <summary>
        /// Agrega una imagen de un agregado(Usando IFormFile).
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddImageAggregateAsync(IFormFile formFile, int AggregateId)
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
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (formFile.Length > Constants.MAX_LENTHG_IMAGE_AGGREGATE)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageAggregateNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ImageAggregateNotValide);
                    return serviceResponse;
                }
                //Obtiene el agregado.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
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
                var imagen_aggregate = new ImageAggregate
                {
                    Image = arraybyte_image,
                    Aggregate = aggregate
                };
                //Guarda la imagen del producto.
                await this._serviceGenericImageAggregateHelper
                .AddEntityAsync(imagen_aggregate)
                .ConfigureAwait(false);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericImageAggregateHelper
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
        /// Agrega una imagen a un agregado.
        /// </summary>
        /// <param name="addImageAggregate"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddImageAggregateAsync(AddImageAggregateDto addImageAggregate)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addImageAggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la imagen, cumpla con los reguerimientos para poderla almacenar en la base de datos.
                if (addImageAggregate.Image == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (addImageAggregate.Image.Length <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                if (addImageAggregate.Image.Length > Constants.MAX_LENTHG_IMAGE_AGGREGATE)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageAggregateNotValide;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ImageAggregateNotValide);
                    return serviceResponse;
                }
                //Obtiene el agregado.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == addImageAggregate.AggregateId)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                var image_product = new ImageAggregate
                {
                   Image = addImageAggregate.Image,
                   Aggregate = aggregate
                };
                await this._serviceGenericImageAggregateHelper
                .AddEntityAsync(image_product)
                .ConfigureAwait(false);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericImageAggregateHelper
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
        /// Borra una imagen de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="ImageId"></param>
        public async Task<ServiceResponse<bool>> DeleteImageAggregateAsync(int AggregateId, int ImageId)
        {
           ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Obtiene la imagen deseada con su producto relacionado.
                var aggregate_image = await this._serviceGenericImageAggregateHelper
                .WhereFirstEntityAsync(c => c.Id == ImageId, c => c.Aggregate)
                .ConfigureAwait(false);
                //Verifica si la imagen existe y si pertenece al producto.
                if(aggregate_image == null || (aggregate_image.Aggregate.Id != AggregateId))
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageNotExist;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageNotExist);
                    return serviceResponse;
                }
                //Elimina la imagen especifica.
                this._serviceGenericImageAggregateHelper.RemoveEntity(aggregate_image);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericImageAggregateHelper
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
        /// Habilita o deshabilita un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> EnableAggregateAsync(int AggregateId, bool enable)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Obtiene el producto.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                if (enable)
                aggregate.IsAvailabe = true;
                else
                aggregate.IsAvailabe = false;
                //Actualiza el producto.
                this._serviceGenericAggregateHelper.UpdateEntity(aggregate);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericAggregateHelper
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
        /// Obtiene todas las imagenes de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetImageAggregateDto>>> GetAllImageAggregateAsync(int AggregateId)
        {
            ServiceResponse<List<GetImageAggregateDto>> serviceResponse = new ServiceResponse<List<GetImageAggregateDto>>();
            try
            {
                //Obtiene el agregado.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId, c => c.Images)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene imagenes.
                if (!aggregate.Images.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImagesNoExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ImagesNoExist);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = aggregate.Images.Select(c => new GetImageAggregateDto
                {
                    Image = c.Image,
                    ImageId = c.Id,
                    AggregateId = c.Aggregate.Id
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
        /// Obtiene todos los agregado en la base de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetAggregateDto>>> GetAllAggregateAsync()
        {
            ServiceResponse<List<GetAggregateDto>> serviceResponse = new ServiceResponse<List<GetAggregateDto>>();
            try
            {
                //Obtiene los productos disponibles
                var all_aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync()
                .ConfigureAwait(false);
                if (!all_aggregate.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_aggregate.Select(c => new GetAggregateDto
                {
                    Id = c.Id,
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
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
        /// Obtiene todos los elementos de todos los agregados
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetAllElementOfAggregateDto>>> GetAggregatesWithAllElement()
        {
            ServiceResponse<List<GetAllElementOfAggregateDto>> serviceResponse = new ServiceResponse<List<GetAllElementOfAggregateDto>>();
            try
            {
                //Obtiene los productos disponibles
                var all_aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Images)
                .ConfigureAwait(false);
                if (all_aggregate == null || all_aggregate.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_aggregate.Select(c => new GetAllElementOfAggregateDto
                {
                    Id = c.Id,
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price, 
                    IsAvailabe = c.IsAvailabe,
                    GetAllImagesAggregate = c.Images.Select(x => new GetImageAggregateDto
                    {
                        Image = x.Image,
                        ImageId = x.Id,
                        AggregateId = c.Id
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
        /// Obtiene todos los elementos de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAllElementOfAggregateDto>> GetAggregateWithAllElement(int AggregateId)
        {
            ServiceResponse<GetAllElementOfAggregateDto> serviceResponse = new ServiceResponse<GetAllElementOfAggregateDto>();
            try
            {
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c=> c.Id == AggregateId, c => c.Images)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAllElementOfAggregateDto
                {
                    Id = aggregate.Id,
                    Description = aggregate.Description,
                    Name = aggregate.Name,
                    Price = aggregate.Price,
                    IsAvailabe = aggregate.IsAvailabe,
                    GetAllImagesAggregate = aggregate.Images.Select(x => new GetImageAggregateDto
                    {
                        Image = x.Image,
                        ImageId = x.Id,
                        AggregateId = aggregate.Id
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
        /// Obtiene una cantidad especifica de imagenes de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="ImageId"></param>
        /// <param name="CantImages"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetImageAggregateDto>>> GetCantImageAggregateAsync(int AggregateId, int ImageId, int CantImages)
        {
            ServiceResponse<List<GetImageAggregateDto>> serviceResponse = new ServiceResponse<List<GetImageAggregateDto>>();
            try
            {
                if (CantImages < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Obtiene el agregado y sus imagenes.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId, c => c.Images)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene imagenes.
                if (!aggregate.Images.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImagesNoExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ImagesNoExist);
                    return serviceResponse;
                }
                //Solicita la cantidad de imagenes deseadas del producto.
                var list_images = this._serviceGenericImageAggregateHelper
                .GetLoadAsync(ImageId, aggregate.Images, CantImages);
                if (!list_images.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageNotExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImageNotExist);
                    return serviceResponse;
                }
                if (list_images.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotNew;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotNew);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = list_images.Select(c => new GetImageAggregateDto
                {
                    Image = c.Image,
                    ImageId = c.Id,
                    AggregateId = c.Aggregate.Id
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
        /// Obtiene una cantidad determinada de agregados dado un agregado de referencia y la cantidad.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="cantAggregate"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetAggregateDto>>> GetCantAggregateAsync(int AggregateId, int cantAggregate)
        {
            ServiceResponse<List<GetAggregateDto>> serviceResponse = new ServiceResponse<List<GetAggregateDto>>();
            try
            {
                if (cantAggregate < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Obtiene el agregado.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                var all_cant_aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(aggregate, cantAggregate)
                .ConfigureAwait(false);
                if (!all_cant_aggregate.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateAllNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateAllNotFound);
                    return serviceResponse;
                }
                serviceResponse.Data = all_cant_aggregate.Select(c => new GetAggregateDto
                {
                    Id = c.Id,
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
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
        /// Obtiene el Id del último agregado.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<int>> GetIdOfLastAggregateAsync()
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();
            try
            {
                var last_product = await this._serviceGenericAggregateHelper
                .LastEntityAsync()
                .ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = last_product.Id;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = -1;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una imagen determinada de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetImageAggregateDto>> GetImageAggregateForIdAsync(int AggregateId, int ImageId)
        {
            ServiceResponse<GetImageAggregateDto> serviceResponse = new ServiceResponse<GetImageAggregateDto>();
            try
            {
                //Obtiene el producto.
                var image_aggregate = await this._serviceGenericImageAggregateHelper
                .WhereFirstEntityAsync(c => c.Id == ImageId, c => c.Aggregate)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if (image_aggregate == null || image_aggregate.Image == null || (image_aggregate.Aggregate.Id != AggregateId))
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImageNotExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ImageNotExist);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetImageAggregateDto
                {
                    Image = image_aggregate.Image,
                    AggregateId = image_aggregate.Aggregate.Id,
                    ImageId = image_aggregate.Id,
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
        /// Devuelve una lista con los Id de todas las imagenes del agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<int>>> GetListIdOfImageAggregateAsync(int AggregateId)
        {
            ServiceResponse<List<int>> serviceResponse = new ServiceResponse<List<int>>();
            try
            {
                //Obtiene el producto.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId, c => c.Images)
                .ConfigureAwait(false);
                //Verifica si el producto existe.
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                //Verifica si el producto tiene imagenes.
                if (aggregate.Images == null || aggregate.Images.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ImagesNoExist;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.ImagesNoExist);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = aggregate.Images.Select(c => c.Id).ToList();
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
        /// Obtiene un agregado dado su Id.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAggregateDto>> GetAggregateForIdAsync(int AggregateId)
        {
            ServiceResponse<GetAggregateDto> serviceResponse = new ServiceResponse<GetAggregateDto>();
            try
            {
                //Obtiene el producto.
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAggregateDto
                {
                    Id = aggregate.Id,
                    Description = aggregate.Description,
                    Name = aggregate.Name,
                    Price = aggregate.Price,
                    IsAvailabe = aggregate.IsAvailabe,
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
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
        /// Actualiza un agregado.
        /// </summary>
        /// <param name="updateAggregate"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAggregateDto>> UpdateAggregateAsync(UpdateAggregateDto updateAggregate)
        {
            ServiceResponse<GetAggregateDto> serviceResponse = new ServiceResponse<GetAggregateDto>();
            try
            {
                if (updateAggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Obtiene el producto que se desea actualizar
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == updateAggregate.AggregateId)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                //Actualiza los campos del producto.
                if (updateAggregate.IsAvailabe != null)
                aggregate.IsAvailabe = (bool)updateAggregate.IsAvailabe;
                if (updateAggregate.Description != null)
                aggregate.Description = updateAggregate.Description;
                if (updateAggregate.Name != null)
                aggregate.Name = updateAggregate.Name;
                if (updateAggregate.Price != null)
                {
                    if (updateAggregate.Price < 1)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                        return serviceResponse;
                    }
                    aggregate.Price = (decimal)updateAggregate.Price; 
                }
                if (updateAggregate.Stock != null)
                {
                    if (updateAggregate.Stock < 1)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                        serviceResponse.Data = null;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                        return serviceResponse;
                    }
                    aggregate.Stock = (int)updateAggregate.Stock; 
                }
                aggregate.DateUpdate = DateTime.UtcNow;
                //Actualiza la entidad
                this._serviceGenericAggregateHelper
                .UpdateEntity(aggregate);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericAggregateHelper
                .SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAggregateDto
                {
                    Description = aggregate.Description,
                    Id = aggregate.Id,
                    Name = aggregate.Name,
                    Price = aggregate.Price
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
        /// Obtiene un agregado dado su Id si el mismo está disponible.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAggregateDto>> GetAggregateIsAvailableForIdAsync(int AggregateId)
        {
            ServiceResponse<GetAggregateDto> serviceResponse = new ServiceResponse<GetAggregateDto>();
            try
            {
                //Obtiene el producto.
                var aggregate = await this._serviceGenericAggregateHelper
                .WhereFirstEntityAsync(c => c.Id == AggregateId && c.IsAvailabe == true)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotIsAvailable);
                    return serviceResponse;
                }
                //Verifica si el producto está disponible
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAggregateDto
                {
                    Id = aggregate.Id,
                    Description = aggregate.Description,
                    Name = aggregate.Name,
                    Price = aggregate.Price,
                    IsAvailabe = aggregate.IsAvailabe,
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
        /// Obtiene todos los agregados disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetAggregateDto>>> GetAllAggregateIsAvailableAsync()
        {
            ServiceResponse<List<GetAggregateDto>> serviceResponse = new ServiceResponse<List<GetAggregateDto>>();
            try
            {
                //Obtiene los productos disponibles
                var all_aggregate = await this._serviceGenericAggregateHelper
                .WhereListEntityAsync(c => c.IsAvailabe == true)
                .ConfigureAwait(false);
                if (!all_aggregate.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductAllNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductAllNotIsAvailable);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = all_aggregate.Select(c => new GetAggregateDto
                {
                    Id = c.Id,
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
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
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Obtiene una cantidad determinada de agregados disponibles dado un agregado de referencia y la cantidad.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="cantAggregate"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetAggregateDto>>> GetCantAggregateIsAvailableAsync(int AggregateId, int cantAggregate)
        {
            ServiceResponse<List<GetAggregateDto>> serviceResponse = new ServiceResponse<List<GetAggregateDto>>();
            try
            {
                if (cantAggregate < 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Obtiene el producto.
                var aggregate = await this._serviceGenericAggregateHelper
                .WhereFirstEntityAsync(c => c.Id == AggregateId && c.IsAvailabe == true)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotIsAvailable;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotIsAvailable);
                    return serviceResponse;
                }
                var all_cant_aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(aggregate, cantAggregate, c => c.IsAvailabe == true)
                .ConfigureAwait(false);
                if (!all_cant_aggregate.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotNew;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotNew);
                    return serviceResponse;
                }
                serviceResponse.Data = all_cant_aggregate.Select(c => new GetAggregateDto
                {
                    Id = c.Id,
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    IsAvailabe = c.IsAvailabe,
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
        /// Borra un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteAggregateAsync(int AggregateId)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var aggregate = await this._serviceGenericAggregateHelper
                .GetLoadAsync(c => c.Id == AggregateId)
                .ConfigureAwait(false);
                if (aggregate == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                    return serviceResponse;
                }
                this._serviceGenericAggregateHelper.RemoveEntity(aggregate);
                //Guarda los cambios en la base de datos.
                await this._serviceGenericAggregateHelper
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
