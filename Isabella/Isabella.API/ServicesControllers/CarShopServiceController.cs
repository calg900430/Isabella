namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;

    using Common;
    using Common.Dtos.CarShop;
    using Common.Extras;
    using Common.RepositorysDtos;
    using RepositorysModels;
    using Models;
    using Extras;

    /// <summary>
    /// Servicio para el controlador del carrito de compras.
    /// </summary>
    public class CarShopServiceController : ICarShopDto
    {
        private readonly IProductStandardRepositoryModel _productStandardRepositoryModel;
        private readonly ICarShopRepositoryModel _carShopRepositoryModel;
        private readonly ICodeIdentificationModel _verificationCode;
        private readonly IProductSpecialRepositoryModel _productSpecialRepositoryModel;
        private readonly IProductAggregateRepositoryModel _productAggregateRepositoryModel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productStandardRepositoryModel"></param>
        /// <param name="carShopRepositoryModel"></param>
        /// <param name="verificationCode"></param>
        /// <param name="productSpecialRepositoryModel"></param>
        /// <param name="productAggregateRepositoryModel"></param>
        public CarShopServiceController(IProductStandardRepositoryModel productStandardRepositoryModel,
        ICarShopRepositoryModel carShopRepositoryModel, ICodeIdentificationModel verificationCode,
        IProductSpecialRepositoryModel productSpecialRepositoryModel, IProductAggregateRepositoryModel productAggregateRepositoryModel)
        {
            this._productStandardRepositoryModel = productStandardRepositoryModel;
            this._carShopRepositoryModel = carShopRepositoryModel;
            this._verificationCode = verificationCode;
            this._productSpecialRepositoryModel = productSpecialRepositoryModel;
            this._productAggregateRepositoryModel = productAggregateRepositoryModel;
        }

        /// <summary>
        /// Agrega productos al carrito de compras.
        /// </summary>
        /// <param name="addProductsToCarShop"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductsToCarShop(AddProductStandardToCarShopDto addProductsToCarShop)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addProductsToCarShop == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica si el código de verificación está disponible
                var codeverification = await this._verificationCode
                .CheckCodeIdentificationAsync(addProductsToCarShop.Verification)
                .ConfigureAwait(false);
                if(codeverification == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeIdentification_NotCode;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeIdentification_NotCode);
                    return serviceResponse;
                }
                //Verifica si el producto standard está disponible 
                var product = await this._productStandardRepositoryModel
                .GetProductStandardForIdNotIncludeAsync(addProductsToCarShop.ProductStandardId)
                .ConfigureAwait(false);
                if(product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //TODO: Verifica que la cantidad de productos solicitados este disponible
                //TODO: Verificar que los productos esten disponibles
                //Agrega un nuevo producto standard al carrito de compras
                var productstandard_for_carshop = new CarShop
                {
                    CodeVerification = codeverification,
                    RequestedProductStandard = new RequestedProductStandard
                    {
                        Price = product.Price,
                        ProductStandard = product,
                        Quantity = addProductsToCarShop.Quantity,
                    },
                    RequestedProductSpecial = null,
                };
                await this._carShopRepositoryModel
                .AddProductsCarShopAsync(productstandard_for_carshop)
                .ConfigureAwait(false);
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch(Exception)
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Agrega productos al carrito de compras.
        /// </summary>
        /// <param name="addProductsToCarShop"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddProductsToCarShop(AddProductSpecialToCarShopDto addProductsToCarShop)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addProductsToCarShop == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_NullObjectSend;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_NullObjectSend);
                    return serviceResponse;
                }
                //Verifica si el código de verificación está disponible
                var codeverification = await this._verificationCode
                .CheckCodeIdentificationAsync(addProductsToCarShop.Verification)
                .ConfigureAwait(false);
                if (codeverification == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeIdentification_NotCode;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeIdentification_NotCode);
                    return serviceResponse;
                }
                //Verifica si el producto standard está disponible 
                var product = await this._productSpecialRepositoryModel
                .GetProductSpecialForIdNotIncludeAsync(addProductsToCarShop.ProductSpecialId)
                .ConfigureAwait(false);
                if (product == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                    return serviceResponse;
                }
                //TODO: Verifica que la cantidad de productos solicitados este disponible
                //TODO: Verificar que los productos esten disponibles
                //Verifica que los productos de agregados esten todos disponibles.
                List<CantRequestProductAggregate> productAggregates = new List<CantRequestProductAggregate>();
                if(addProductsToCarShop.CantProductAggregates != null && addProductsToCarShop.CantProductAggregates.Count != 0)
                {
                    foreach(CantProductAggregate cantProductAggregate in addProductsToCarShop.CantProductAggregates)
                    {
                        var product_aggregate = await this._productAggregateRepositoryModel
                        .GetProductAggregateForIdNotIncludeAsync(cantProductAggregate.ProductAggregateId)
                        .ConfigureAwait(false);
                        if(product_aggregate == null)
                        {
                            serviceResponse.Code = CodeMessage.Code.CodeProduct_NotFound;
                            serviceResponse.Data = false;
                            serviceResponse.Success = false;
                            serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeProduct_NotFound);
                            return serviceResponse;
                        }
                        var new_cant_product_aggregate = new CantRequestProductAggregate
                        {
                            ProductAggregate = product_aggregate,
                            Quantity = cantProductAggregate.Quantity
                        };
                        productAggregates.Add(new_cant_product_aggregate);
                    }
                }
                //Agrega un nuevo producto standard al carrito de compras
                var productstandard_for_carshop = new CarShop
                {
                    CodeVerification = codeverification,
                    RequestedProductSpecial = new RequestedProductSpecial
                    {
                        CheeseGouda = addProductsToCarShop.CheeseGouda,
                        Price = product.Price,
                        ProductSpecial = product,
                        Quantity = addProductsToCarShop.Quantity,
                        RequestedProductAggregates = productAggregates.Select( c => new RequestedProductAggregate 
                        { 
                            Price = c.ProductAggregate.Price,
                            ProductAggregate = c.ProductAggregate,
                            Quantity = c.Quantity,
                        }).ToList(),
                    },
                    RequestedProductStandard = null,
                };
                await this._carShopRepositoryModel
                .AddProductsCarShopAsync(productstandard_for_carshop)
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
    }
}
