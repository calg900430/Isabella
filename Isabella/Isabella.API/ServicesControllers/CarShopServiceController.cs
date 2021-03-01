namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Common;
    using Common.Dtos.CarShop;
    using Common.RepositorysDtos;
    using Helpers.RepositoryHelpers;
    using Helpers;
    using Models.Entities;
    using Resources;
    using AutoMapper;
    using Isabella.Common.Dtos.SubCategory;

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
        private readonly ServiceGenericHelper<SubCategory> _serviceGenericSubCategoryHelper;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceGenericCarShopHelper"></param>
        /// <param name="serviceGenericProductHelper"></param>
        /// <param name="serviceGenericAggregateHelper"></param>
        /// <param name="serviceGenericCodeIdentificationHelper"></param>
        /// <param name="serviceGenericCantAggregateHelper"></param>
        /// <param name="serviceGenericSubCategoryHelper"></param>
        /// <param name="mapper"></param>
        public CarShopServiceController(ServiceGenericHelper<CarShop> serviceGenericCarShopHelper, 
        ServiceGenericHelper<Product> serviceGenericProductHelper, 
        ServiceGenericHelper<Aggregate> serviceGenericAggregateHelper,
        ServiceGenericHelper<CodeIdentification> serviceGenericCodeIdentificationHelper,
        ServiceGenericHelper<CantAggregate> serviceGenericCantAggregateHelper,
        ServiceGenericHelper<SubCategory> serviceGenericSubCategoryHelper,
        IMapper mapper)
        {
            this._serviceGenericCarShopHelper = serviceGenericCarShopHelper;
            this._serviceGenericProductHelper = serviceGenericProductHelper;
            this._serviceGenericAggregateHelper = serviceGenericAggregateHelper;
            this._serviceGenericCodeIdentificationHelper = serviceGenericCodeIdentificationHelper;
            this._serviceGenericCantAggregateHelper = serviceGenericCantAggregateHelper;
            this._serviceGenericSubCategoryHelper = serviceGenericSubCategoryHelper;
            this._mapper = mapper;
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
                if (addProductsToCarShop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.EntityIsNull;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull);
                    return serviceResponse;
                }
                //Verifica que la cantidad se mayor que 1.
                if(addProductsToCarShop.Quantity < 1)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                    return serviceResponse;
                }
                //Verifica si el código de verificación está disponible
                var code_identification = await this._serviceGenericCodeIdentificationHelper
                .WhereSingleEntityAsync(c => c.Code == addProductsToCarShop.CodeIdentification)
                .ConfigureAwait(false);
                if(code_identification == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.NotCodeIdentification;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.NotCodeIdentification);
                    return serviceResponse;
                }
                //Verifica si el producto está en la base de datos y está disponible.
                var product = await this._serviceGenericProductHelper
                .WhereFirstEntityAsync(c => c.Id == addProductsToCarShop.ProductId && c.IsAvailabe == true)
                .ConfigureAwait(false);
                if(product == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotIsAvailable;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotIsAvailable);
                    return serviceResponse;
                }
                //Verifica si el usuario pide el producto con subcategoria
                var subCategory = new SubCategory();
                if(addProductsToCarShop.SubCategoryId != null)
                {
                    //Verifica si la subcategoria existe y pertenece al producto
                    subCategory = await this._serviceGenericSubCategoryHelper
                    .WhereFirstEntityAsync(c => c.Id == addProductsToCarShop.SubCategoryId && c.Product == product, c => c.Product)
                    .ConfigureAwait(false);
                    if (subCategory == null)
                    {
                        serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SubCategoryNotIsProduct;
                        serviceResponse.Data = false;
                        serviceResponse.Success = false;
                        serviceResponse.Message = GetValueResourceFile
                        .GetValueResourceString(GetValueResourceFile.KeyResource.SubCategoryNotIsProduct);
                        return serviceResponse;
                    }
                }
                else
                subCategory = null;
                //Verifica si el producto seleccionado admite agregados
                if (product.SupportAggregate == false && addProductsToCarShop.CantAggregates != null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ProductNotSupportAggregate;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ProductNotSupportAggregate);
                    return serviceResponse;
                }
                //Verifica que los agregados solicitados esten disponibles.
                List<Models.Entities.CantAggregate> cantAggregates = new List<Models.Entities.CantAggregate>();
                if (product.SupportAggregate == true && addProductsToCarShop.CantAggregates != null)
                {
                   foreach(string key in addProductsToCarShop.CantAggregates.Keys)
                   {
                      int parse_key;
                      if(!int.TryParse(key, out parse_key))
                      {
                         serviceResponse.Code = (int)GetValueResourceFile.KeyResource.FormatAggregateNotSupport;
                         serviceResponse.Data = false;
                         serviceResponse.Success = false;
                         serviceResponse.Message = GetValueResourceFile
                         .GetValueResourceString(GetValueResourceFile.KeyResource.FormatAggregateNotSupport);
                         return serviceResponse;
                      }
                      if(parse_key < 1)
                      {
                         serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                         serviceResponse.Data = false;
                         serviceResponse.Success = false;
                         serviceResponse.Message = GetValueResourceFile
                         .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                         return serviceResponse;
                      }
                      var quantity = addProductsToCarShop.CantAggregates.GetValueOrDefault(key);
                      if(quantity < 1)
                      { 
                         serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CantIsNegative;
                         serviceResponse.Data = false;
                         serviceResponse.Success = false;
                         serviceResponse.Message = GetValueResourceFile
                         .GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative);
                         return serviceResponse;
                      }
                      var aggregate = await this._serviceGenericAggregateHelper
                      .WhereFirstEntityAsync(c => c.Id == parse_key && c.IsAvailabe == true)
                      .ConfigureAwait(false);
                      if (aggregate == null)
                      {
                         serviceResponse.Code = (int)GetValueResourceFile.KeyResource.AggregateNotIsAvailable;
                         serviceResponse.Data = false;
                         serviceResponse.Success = false;
                         serviceResponse.Message = GetValueResourceFile
                         .GetValueResourceString(GetValueResourceFile.KeyResource.AggregateNotIsAvailable);
                         return serviceResponse;
                      }
                      //TODO: Verifica que la cantidad de agregados solicitados este disponible
                      var new_aggregate = new Models.Entities.CantAggregate
                      {
                         Aggregate = aggregate,
                         Price = aggregate.Price,
                         Quantity = quantity,
                      };
                      cantAggregates.Add(new_aggregate);
                   }
                }
                //Verifica si el producto se ha pedido con subcategoria.
                decimal new_price = 0;
                if (subCategory == null)
                    new_price = product.Price;
                else
                    new_price = subCategory.Price;
                //Verifica si el usuario no tiene este producto en el carrito para agregarlo
                var product_is_cartshop = await this._serviceGenericCarShopHelper._context
                .Include(c => c.Product)
                .Include(c => c.CantAggregates).ThenInclude(c => c.Aggregate)
                .Where(c => c.CodeIdentification == code_identification && c.Product == product)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
                if(product_is_cartshop == null)
                {
                    product_is_cartshop = new CarShop
                    {
                        CodeIdentification = code_identification,
                        Price = new_price,
                        Product = product,
                        SubCategory = subCategory,
                        Quantity = addProductsToCarShop.Quantity,
                        CantAggregates = cantAggregates.Select(c => new Models.Entities.CantAggregate
                        {
                            Aggregate = c.Aggregate,
                            Price = c.Price,
                            Quantity = c.Quantity
                        }).ToList(),
                        CheeseGouda = addProductsToCarShop.ChesseGouda,
                    };
                    //Agrega un nuevo producto al carrito de compras
                    await this._serviceGenericCarShopHelper
                    .AddEntityAsync(product_is_cartshop)
                    .ConfigureAwait(false);
                }
                //Verifica si el usuario tiene el producto en el carrito para actualizar las cantidades 
                else
                {
                    //Actualiza el producto con la nueva cantidad agregada.
                    product_is_cartshop.Quantity += addProductsToCarShop.Quantity;
                    //Verifica si el producto tiene agregados y si hay actualizaciones para los agregados.
                    if ((product_is_cartshop.CantAggregates != null || product_is_cartshop.CantAggregates.Count > 0) &&
                    (cantAggregates != null || cantAggregates.Count > 0))
                    {
                       //Verifica si hay agregados que actualizar

                       //Verifica si hay agregados para agregar
                    }
                    //Para los otros casos asigna cantAggregates
                    else
                    product_is_cartshop.CantAggregates = cantAggregates;
                }
                await this._serviceGenericCarShopHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile
                .GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch(Exception ex)
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
        /// Obtiene el carrito de compras de un usuario.
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAllProductOfMyCarShopDto>> GetMyCarShop(Guid codeIdentification)
        {
            ServiceResponse<GetAllProductOfMyCarShopDto> serviceResponse = new ServiceResponse<GetAllProductOfMyCarShopDto>();
            try
            {
                //Verifica si el código de identificación está disponible
                //Verifica si el código de verificación está disponible
                var code_identification = await this._serviceGenericCodeIdentificationHelper
                .WhereFirstEntityAsync(c => c.Code == codeIdentification)
                .ConfigureAwait(false);
                if (code_identification == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.NotCodeIdentification;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.NotCodeIdentification);
                    return serviceResponse;
                }
                //Verifica si hay productos en el carrito
                var all_products_in_carshop = await this._serviceGenericCarShopHelper._context
                .Include(c => c.SubCategory.Product.Category)
                .Include(c => c.Product).ThenInclude(c => c.Category)
                .Include(c => c.CantAggregates).ThenInclude(c => c.Aggregate)
                .Where(c => c.CodeIdentification == code_identification)
                .ToListAsync()
                .ConfigureAwait(false);
                if (all_products_in_carshop == null || all_products_in_carshop.Count <= 0)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CarShopNotProducts;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CarShopNotProducts);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAllProductOfMyCarShopDto
                {
                   Identification = code_identification.Code,
                   GetCarShopProducts = all_products_in_carshop.Select(c => new GetCarShopProductDto 
                   { 
                      ProductId = c.Product.Id,
                      Average = c.Product.Average,
                      SupportAggregate = c.Product.SupportAggregate,
                      CheeseGouda = c.CheeseGouda,
                      Name = c.Product.Name,
                      Price = c.Price,
                      PriceTotal = c.PriceTotal,
                      Description = c.Product.Description,
                      IsAvailabe = c.Product.IsAvailabe,
                      Quantity = c.Quantity,
                      SubCategory = this._mapper.Map<GetSubCategoryDto>(c.SubCategory),
                      Category = new Common.Dtos.Category.GetCategoryDto
                      {
                          Id = c.Product.Category.Id,
                          Name = c.Product.Category.Name,
                      },
                      CantAggregates = c.CantAggregates.Select(x => new GetCantAggregateDto 
                      { 
                         Id = x.Aggregate.Id,
                         Name = x.Aggregate.Name,
                         Price = x.Price,
                         PriceTotal = x.PriceTotal,
                         Quantity = x.Quantity,
                      }).ToList(),
                   }).ToList(), 
                   PriceTotal = all_products_in_carshop.Sum(c => c.PriceTotal),
                };
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }
    }
}
