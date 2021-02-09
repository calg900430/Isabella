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
                var productstandard_for_carshop = new CarShopProductStandard
                {
                    CodeIdentification = codeverification,
                    Price = product.Price,
                    ProductStandard = product,
                    Quantity = addProductsToCarShop.Quantity,
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
                List<CarShopProductAggregate> productAggregates = new List<CarShopProductAggregate>();
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
                        var new_cant_product_aggregate = new CarShopProductAggregate
                        {
                            ProductAggregate = product_aggregate,
                            Quantity = cantProductAggregate.Quantity,
                            Price = product_aggregate.Price,
                        };
                        productAggregates.Add(new_cant_product_aggregate);
                    }
                }
                //Agrega un nuevo producto standard al carrito de compras
                var productstandard_for_carshop = new CarShopProductSpecial
                {
                    CodeIdentification = codeverification,
                    CarShopProductAggregates = productAggregates.Select(c => new CarShopProductAggregate
                    {
                        Price = c.ProductAggregate.Price,
                        ProductAggregate = c.ProductAggregate,
                        Quantity = c.Quantity,
                    }).ToList(),
                    CheeseGouda = addProductsToCarShop.CheeseGouda,
                    Price = product.Price,
                    ProductSpecial = product,
                    Quantity = addProductsToCarShop.Quantity,
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

        /// <summary>
        /// Obtiene el carrito de compras de un usuario.
        /// </summary>
        /// <param name="CodeVerification"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetCarShopDto>> GetMyCarShop(Guid CodeVerification)
        {
            ServiceResponse<GetCarShopDto > serviceResponse = new ServiceResponse<GetCarShopDto>();
            try
            {
                //Verifica si el código de verificación está disponible
                var codeverification = await this._verificationCode
                .CheckCodeIdentificationAsync(CodeVerification)
                .ConfigureAwait(false);
                if (codeverification == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeIdentification_NotCode;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeIdentification_NotCode);
                    return serviceResponse;
                }
                //Verifica si hay productos standards
                var all_product_standars = await this._carShopRepositoryModel
                .GetMyCarShopProductStandard(codeverification)
                .ConfigureAwait(false);
                //Verifica si hay productos especiales
                var all_product_specials = await this._carShopRepositoryModel
                .GetMyCarShopProductSpecial(codeverification)
                .ConfigureAwait(false);
                //Verifica si el usuario tiene productos en su carrito.
                if(all_product_standars == null && all_product_specials == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeCarShop_NotProducts;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeCarShop_NotProducts);
                    return serviceResponse;
                }
                //EL usuario solicita ambos tipos de productos
                else if(all_product_specials != null && all_product_standars != null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Data = new GetCarShopDto
                    {
                        Verification = codeverification.Code,
                        CarShopProductStandards = all_product_standars.Select(c => new GetCarShopProductStandard
                        {
                            Id = c.Id,
                            Average = c.ProductStandard.Average,
                            Category = new Common.Dtos.Category.GetCategoryDto
                            {
                                Id = c.ProductStandard.Category.Id,
                                Name = c.ProductStandard.Category.Name,
                            },
                            Price = c.Price,
                            Quantity = c.Quantity,
                            Name = c.ProductStandard.Name,
                        }).ToList(),
                        CarShopProductSpecial = all_product_specials.Select(c => new GetCarShopProductSpecial
                        {
                            Id = c.Id,
                            Average = c.ProductSpecial.Average,
                            Category = new Common.Dtos.Category.GetCategoryDto
                            {
                                Id = c.ProductSpecial.Category.Id,
                                Name = c.ProductSpecial.Category.Name,
                            },
                            Price = c.Price,
                            Quantity = c.Quantity,
                            Name = c.ProductSpecial.Name,
                            CheeseGouda = c.CheeseGouda,
                            GetCarShopProductAggregates = c.CarShopProductAggregates.Select(z => new GetCarShopProductAggregate
                            {
                                Category = new Common.Dtos.Category.GetCategoryDto
                                {
                                    Id = z.ProductAggregate.Category.Id,
                                    Name = z.ProductAggregate.Category.Name,
                                },
                                CarShopProductSpecialId = c.Id,
                                Id = z.Id,
                                Name = z.ProductAggregate.Name,
                                Price = z.Price,
                                Quantity = z.Quantity
                            }).ToList()
                        }).ToList()
                    };
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
                //El usuario solicita productos standards
                else if(all_product_standars != null && all_product_specials == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Data = new GetCarShopDto
                    {
                        Verification = codeverification.Code,
                        CarShopProductStandards = all_product_standars.Select(c => new GetCarShopProductStandard
                        {
                            Id = c.Id,
                            Average = c.ProductStandard.Average,
                            Category = new Common.Dtos.Category.GetCategoryDto
                            {
                                Id = c.ProductStandard.Category.Id,
                                Name = c.ProductStandard.Category.Name,
                            },
                            Price = c.Price,
                            Quantity = c.Quantity,
                            Name = c.ProductStandard.Name,
                        }).ToList(),
                    };
                    serviceResponse.Success = true;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                    return serviceResponse;
                }
                //El usuario solicita productos especiales
                else
                {
                    serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                    serviceResponse.Data = new GetCarShopDto
                    {
                        Verification = codeverification.Code,
                        CarShopProductSpecial = all_product_specials.Select(c => new GetCarShopProductSpecial
                        {
                            Id = c.Id,
                            Average = c.ProductSpecial.Average,
                            Category = new Common.Dtos.Category.GetCategoryDto
                            {
                                Id = c.ProductSpecial.Category.Id,
                                Name = c.ProductSpecial.Category.Name,
                            },
                            Price = c.Price,
                            Quantity = c.Quantity,
                            Name = c.ProductSpecial.Name,
                            CheeseGouda = c.CheeseGouda,
                            GetCarShopProductAggregates = c.CarShopProductAggregates.Select(z => new GetCarShopProductAggregate
                            {
                                Category = new Common.Dtos.Category.GetCategoryDto
                                {
                                    Id = z.ProductAggregate.Category.Id,
                                    Name = z.ProductAggregate.Category.Name,
                                },
                                CarShopProductSpecialId = c.Id,
                                Id = z.Id,
                                Name = z.ProductAggregate.Name,
                                Price = z.Price,
                                Quantity = z.Quantity
                            }).ToList()
                        }).ToList()
                    };
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
    }
}
