namespace Isabella.Web.ServicesControllers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Isabella.Web.Helpers;
    using Isabella.Web.Helpers.RepositoryHelpers;
    using Isabella.Web.Models.Entities;
    using Isabella.Web.Resources;
    using Isabella.Common;
    using Isabella.Common.Dtos.Resturant;
    using Isabella.Common.RepositorysDtos;

    /// <summary>
    /// Servicio para el controlador del restaurante.
    /// </summary>
    public class RestaurantServiceController : IRestaurantRepositoryDto
    {
        private readonly ServiceGenericHelper<Restaurant> _serviceGenericRestaurantHelper;
        private readonly ServiceGenericHelper<CartShop> _serviceGenericCartShopHelper;
        private readonly ServiceGenericHelper<CantAggregate> _serviceGenericCantAggregateHelper;
        private readonly ServiceGenericHelper<ProductCombined> _serviceGenericProductCombinedHelper;
        private readonly IUserRepositoryHelper _userRepositoryHelper;

        /// <summary>
        /// Claims del usuario.
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceGenericRestaurantHelper"></param>
        /// <param name="serviceGenericCarShopHelper"></param>
        /// <param name="serviceGenericCantAggregateHelper"></param>
        /// <param name="serviceGenericProductCombinedHelper"></param>
        /// <param name="userRepositoryHelper"></param>
        public RestaurantServiceController(ServiceGenericHelper<Restaurant> serviceGenericRestaurantHelper,
        ServiceGenericHelper<CartShop> serviceGenericCarShopHelper,
        ServiceGenericHelper<CantAggregate> serviceGenericCantAggregateHelper,
        ServiceGenericHelper<ProductCombined> serviceGenericProductCombinedHelper,
        IUserRepositoryHelper userRepositoryHelper)
        {
            this._serviceGenericRestaurantHelper = serviceGenericRestaurantHelper;
            this._serviceGenericCartShopHelper = serviceGenericCarShopHelper;
            this._userRepositoryHelper = userRepositoryHelper;
            this._serviceGenericCantAggregateHelper = serviceGenericCantAggregateHelper;
            this._serviceGenericProductCombinedHelper = serviceGenericProductCombinedHelper;
        }

       

        /// <summary>
        /// Verifica el estado del resturante.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> GetStateRestaurantAsync()
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try 
            {
                var state_restaurant = await this._serviceGenericRestaurantHelper.WhereFirstEntityAsync(c => c.Id == 1).ConfigureAwait(false);
                if(state_restaurant == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RestaurantError;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.RestaurantError);
                    return serviceResponse;
                }
                if(state_restaurant.IsOpenRestaurant)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RestaurantIsOpen;
                    serviceResponse.Data = true;
                    serviceResponse.Success = true;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.RestaurantIsOpen);
                    return serviceResponse;

                }
                else
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RestaurantIsClose;
                    serviceResponse.Data = false;
                    serviceResponse.Success = true;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.RestaurantIsClose);
                    return serviceResponse;
                }
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
        /// Cierra el restaurante.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> CloseRestautantAsync()
        {
           ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
           try
           {
                var restaurant = await this._serviceGenericRestaurantHelper
                .WhereFirstEntityAsync(c => c.Id == 1).ConfigureAwait(false);
                if (restaurant == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RestaurantError;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.RestaurantError);
                    return serviceResponse;
                }
                restaurant.IsOpenRestaurant = false;
                this._serviceGenericRestaurantHelper.UpdateEntity(restaurant);
                await this._serviceGenericRestaurantHelper.SaveChangesBDAsync().ConfigureAwait(false);
                //Elimina todos los productos que estén en los carritos de los usuarios.
                 var all_carshop = await this._serviceGenericCartShopHelper
                .WhereListEntityAsync(c => c.Id > 0, c => c.ProductCombined.CantAggregates, c => c.User)
                .ConfigureAwait(false);
                if (all_carshop == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CarShopNotProducts;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CarShopNotProducts);
                    return serviceResponse;
                }
                //Elimina la cantidad de agregados que tiene cada producto combinado como referencia
                foreach(CartShop carshop in all_carshop)
                {
                    var all_cantaggregate_of_productcombined = await this._serviceGenericCantAggregateHelper
                    .WhereListEntityAsync(c => c.ProductCombined == carshop.ProductCombined, c => c.ProductCombined)
                    .ConfigureAwait(false);
                    if(all_cantaggregate_of_productcombined != null)
                    {
                        if (all_cantaggregate_of_productcombined.Any())
                        {
                            //Elimina los agregados
                            this._serviceGenericCantAggregateHelper.RemoveRangeEntity(all_cantaggregate_of_productcombined);
                            await this._serviceGenericCantAggregateHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        }
                    }
                    //Elimina el producto combinado
                    this._serviceGenericProductCombinedHelper.RemoveEntity(carshop.ProductCombined);
                    await this._serviceGenericProductCombinedHelper
                    .SaveChangesBDAsync().ConfigureAwait(false);
                    //Elimina la relación del producto combinado con el carrito
                    this._serviceGenericCartShopHelper.RemoveEntity(carshop);
                    await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RestaurantClose;
                serviceResponse.Data = false;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.
                GetValueResourceString(GetValueResourceFile.KeyResource.RestaurantClose);
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
        /// Abre el restaurante.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> OpenRestaurantAsync()
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var restaurant = await this._serviceGenericRestaurantHelper
                .WhereFirstEntityAsync(c => c.Id == 1).ConfigureAwait(false);
                if (restaurant == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RestaurantError;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.RestaurantError);
                    return serviceResponse;
                }
                restaurant.IsOpenRestaurant = true;
                this._serviceGenericRestaurantHelper.UpdateEntity(restaurant);
                await this._serviceGenericRestaurantHelper.SaveChangesBDAsync().ConfigureAwait(false);
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.RestaurantOpen;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.
                GetValueResourceString(GetValueResourceFile.KeyResource.RestaurantOpen);
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
    }
}
