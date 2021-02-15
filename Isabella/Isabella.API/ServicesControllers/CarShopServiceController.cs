namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Common;
    using Common.Dtos.CarShop;
    using Common.Extras;
    using Common.RepositorysDtos;
    using Helpers.RepositoryHelpers;
    using Helpers;
    using Models.Entities;
   

    /// <summary>
    /// Servicio para el controlador del carrito de compras.
    /// </summary>
    public class CarShopServiceController : ICarShopDto
    {

        private readonly ServiceGenericHelper<CarShop> _serviceGenericCarShopHelper;
        private readonly ServiceGenericHelper<Product> _serviceGenericProductHelper;
        private readonly ServiceGenericHelper<Aggregate> _serviceGenericAggregateHelper;
        private readonly ServiceGenericHelper<CodeIdentification> _serviceGenericCodeIdentificationHelper;
        private readonly ServiceGenericHelper<CantAggregate> _serviceGenericCantAggregateHelper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceGenericCarShopHelper"></param>
        /// <param name="serviceGenericProductHelper"></param>
        /// <param name="serviceGenericAggregateHelper"></param>
        /// <param name="serviceGenericCodeIdentificationHelper"></param>
        /// <param name="serviceGenericCantAggregateHelper"></param>
        public CarShopServiceController(ServiceGenericHelper<CarShop> serviceGenericCarShopHelper, 
        ServiceGenericHelper<Product> serviceGenericProductHelper, 
        ServiceGenericHelper<Aggregate> serviceGenericAggregateHelper,
        ServiceGenericHelper<CodeIdentification> serviceGenericCodeIdentificationHelper,
        ServiceGenericHelper<CantAggregate> serviceGenericCantAggregateHelper)
        {
            this._serviceGenericCarShopHelper = serviceGenericCarShopHelper;
            this._serviceGenericProductHelper = serviceGenericProductHelper;
            this._serviceGenericAggregateHelper = serviceGenericAggregateHelper;
            this._serviceGenericCodeIdentificationHelper = serviceGenericCodeIdentificationHelper;
            this._serviceGenericCantAggregateHelper = serviceGenericCantAggregateHelper;
        }

        /// <summary>
        /// Agrega productos al carrito de compras.
        /// </summary>
        /// <param name="addProductsToCarShop"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductsToCarShop(AddProductToCarShopDto addProductsToCarShop)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Manejar la cantidad de productos
                if (addProductsToCarShop == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica si el código de verificación está disponible
                var code_identification = await this._serviceGenericCodeIdentificationHelper
                .WhereSingleEntityAsync(c => c.Code == addProductsToCarShop.CodeIdentification)
                .ConfigureAwait(false);
                if(code_identification == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.NotCodeIdentification;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.NotCodeIdentification);
                    return serviceResponse;
                }
                //Verifica si el producto está en la base de datos.
                var product = await this._serviceGenericProductHelper
                .WhereFirstEntityAsync(c => c.Id == addProductsToCarShop.ProductId)
                .ConfigureAwait(false);
                if(product == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.ProductNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotFound);
                    return serviceResponse;
                }
                //TODO: Verificar que el producto este disponible.
                //TODO: Verifica que la cantidad de productos solicitados este disponibles.
                //Verifica que los agregados solicitados esten en la base de datos.
                List<Models.Entities.CantAggregate> cantAggregates = new List<Models.Entities.CantAggregate>();
                foreach (string key in addProductsToCarShop.CantAggregates.Keys)
                {      
                    var aggregate = await this._serviceGenericAggregateHelper
                    .WhereFirstEntityAsync(c => c.Id == int.Parse(key), c => c.Category)
                    .ConfigureAwait(false);
                    if(aggregate == null)
                    {
                        serviceResponse.KeyResource = GetValueResourceFile.KeyResource.AggregateNotFound;
                        serviceResponse.Data = false;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotFound);
                        return serviceResponse;
                    }
                    //TODO: Verifica que la cantidad de agregados solicitados este disponible
                    var new_aggregate = new Models.Entities.CantAggregate
                    {
                        Aggregate = aggregate,
                        Price = aggregate.Price,
                        Quantity = addProductsToCarShop.CantAggregates.GetValueOrDefault(key)
                    };
                    cantAggregates.Add(new_aggregate);
                }
                //Si el usuario no tiene este producto en el carrito lo agrega

                //Si el usuario tiene el producto en el carrito actualiza suma las cantidades
                //Agrega un nuevo producto al carrito de compras
                var new_product_to_carshop = new CarShop
                {
                    CodeIdentification = code_identification,
                    Price = product.Price,
                    Product = product,
                    Quantity = addProductsToCarShop.Quantity,
                    CantAggregates = cantAggregates.Select(c =>new Models.Entities.CantAggregate 
                    { 
                        Aggregate = c.Aggregate,
                        Price = c.Price,
                        Quantity =  c.Quantity  
                    }).ToList(),
                    CheeseGouda = addProductsToCarShop.ChesseGouda,
                };
                await this._serviceGenericCarShopHelper
                .AddEntityAsync(new_product_to_carshop)
                .ConfigureAwait(false);
                await this._serviceGenericCarShopHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

     
        /// <summary>
        /// Obtiene el carrito de compras de un usuario.
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAllPorductOfMyCarShopDto>> GetMyCarShop(Guid codeIdentification)
        {
            ServiceResponse<GetAllPorductOfMyCarShopDto> serviceResponse = new ServiceResponse<GetAllPorductOfMyCarShopDto>();
            try
            {
                //Verifica si el código de identificación está disponible
                //Verifica si el código de verificación está disponible
                var code_identification = await this._serviceGenericCodeIdentificationHelper
                .WhereSingleEntityAsync(c => c.Code == codeIdentification)
                .ConfigureAwait(false);
                if (code_identification == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.NotCodeIdentification;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.NotCodeIdentification);
                    return serviceResponse;
                }
                //Verifica si hay productos en el carrito
                var all_products = await this._serviceGenericCarShopHelper._context
                .Include(c => c.Product).ThenInclude(c => c.Category)
                .Include(c => c.CantAggregates)
                .ThenInclude(x => x.Aggregate.Category)
                .Where(c => c.CodeIdentification == code_identification)
                .ToListAsync()
                .ConfigureAwait(false);
                if (all_products == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.CarShopNotProducts;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CarShopNotProducts);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAllPorductOfMyCarShopDto
                {
                    Identification = code_identification.Code,
                    GetCarShopProducts = all_products.Select(c => new GetCarShopProductDto 
                    { 
                       GetProductDto = new Common.Dtos.Product.GetProductDto 
                       { 
                          Average = c.Product.Average,
                          Category = new Common.Dtos.Category.GetCategoryDto
                          {
                              Id = c.Product.Category.Id,
                              Name = c.Product.Category.Name,
                          },
                          Description = c.Product.Description,
                          Name = c.Product.Name,
                          Id = c.Product.Id,
                          Price = c.Product.Price
                       },
                       CheeseGouda = c.CheeseGouda,
                       Quantity = c.Quantity,
                       CantAggregates = c.CantAggregates.Select(x => new GetCantAggregateDto
                       { 
                           Category = new Common.Dtos.Category.GetCategoryDto
                           {
                              Id = x.Aggregate.Category.Id,
                              Name = x.Aggregate.Category.Name
                           },
                           Id = x.Id,
                           Price = x.Price,
                           Name = x.Aggregate.Name,
                           Quantity = x.Quantity
                       }).ToList(),
                    }).ToList(),  
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception)
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }
    }
}
