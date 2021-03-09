namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    using Common;
    using Common.Dtos.Order;
    using Common.RepositorysDtos;
    using Helpers;
    using Models.Entities;
    using Resources;
    using AutoMapper;
    using Isabella.Common.Dtos.SubCategory;
    using Isabella.Common.Dtos.CarShop;

    /// <summary>
    /// Servicio para las ordenes
    /// </summary>
    public class OrderServiceController : IRepositoryOrderDto
    {
        private readonly ServiceGenericHelper<Order> _serviceGenericOrderHelper;
        private readonly ServiceGenericHelper<OrderDetail> _serviceGenericOrderDetailHelper;
        private readonly ServiceGenericHelper<CodeIdentification> _serviceGenericCodeIdentificationHelper;
        private readonly ServiceGenericHelper<CartShop> _serviceGenericCartShopHelper;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceGenericOrderHelper"></param>
        /// <param name="serviceGenericOrderDetailHelper"></param>
        /// <param name="serviceGenericCodeIdentificationHelper"></param>
        /// <param name="serviceGenericCartShopHelper"></param>
        /// <param name="mapper"></param>
        public OrderServiceController(ServiceGenericHelper<Order> serviceGenericOrderHelper, 
        ServiceGenericHelper<OrderDetail> serviceGenericOrderDetailHelper, 
        ServiceGenericHelper<CodeIdentification> serviceGenericCodeIdentificationHelper,
        ServiceGenericHelper<CartShop> serviceGenericCartShopHelper,
        IMapper mapper)
        {
            this._serviceGenericOrderHelper = serviceGenericOrderHelper;
            this._serviceGenericOrderDetailHelper = serviceGenericOrderDetailHelper;
            this._serviceGenericCodeIdentificationHelper = serviceGenericCodeIdentificationHelper;
            this._serviceGenericCartShopHelper = serviceGenericCartShopHelper;
            this._mapper = mapper;
        }

        /// <summary>
        /// Obtiene todas las ordenes del usuario.
        /// </summary>
        /// <param name="confirmOrder"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ConfirmOrderAsync(ConfirmOrderDto confirmOrder)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica si el código de verificación está disponible
                var code_identification = await this._serviceGenericCodeIdentificationHelper
                .WhereFirstEntityAsync(c => c.Code == confirmOrder.CodeVerification)
                .ConfigureAwait(false);
                if (code_identification == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.NotCodeIdentification;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.NotCodeIdentification);
                    return serviceResponse;
                }
                //Obtiene todos los productos del carrito del usuario
                var all_products_in_carshop = await this._serviceGenericCartShopHelper._context
                .Include(c => c.CodeIdentification)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.Product.SubCategories)
                .Include(c => c.ProductCombined.SubCategory.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Include(c => c.ProductCombined.CantAggregates)
                .Where(c => c.CodeIdentification == code_identification)
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync()
                .ConfigureAwait(false);
                //Guarda los detalles de las ordenes
                if (!all_products_in_carshop.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.CarShopNotProducts;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.CarShopNotProducts);
                    return serviceResponse;
                }
                var ordersdetails = all_products_in_carshop.Select(c => new OrderDetail
                {
                   DateCreated = c.DateCreated,
                   PriceProductCombined = c.PriceProductCombined,
                   PriceTotalOfAggregates = c.PriceTotalOfAggregates,
                   PriceTotalProductCombined = c.PriceTotalProductCombined,
                   QuantityProductCombined = c.QuantityProductCombined,
                   QuantityTotalAggregate = c.QuantityTotalAggregate,
                   ProductCombined = c.ProductCombined,
                }).ToList();
                //Crea la orden
                var order = new Order
                {
                    OrderDetails = ordersdetails,
                    Address = confirmOrder.Address,
                    AskForWho = confirmOrder.AskForWho,
                    Gps = this._mapper.Map<Gps>(confirmOrder.AddGps),
                    CodeVerification = code_identification,
                    DeliveryDate = DateTime.UtcNow,
                    OrderDate = DateTime.UtcNow,
                    PhoneNumber = confirmOrder.PhoneNumber,
                };
                //Guarda la orden
                await this._serviceGenericOrderHelper.AddEntityAsync(order).ConfigureAwait(false);
                //Guarda los cambios en la base de datos
                await this._serviceGenericOrderHelper.SaveChangesBDAsync().ConfigureAwait(false);
                //Elimina los productos del carrito
                this._serviceGenericCartShopHelper.RemoveRangeEntity(all_products_in_carshop);
                //Guarda los cambios en la base de datos
                await this._serviceGenericCartShopHelper.SaveChangesBDAsync().ConfigureAwait(false);
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
        /// Confirma las ordenes de un usuario.
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAllOrderDto>> GetAllOrderAsync(Guid codeIdentification)
        {
            ServiceResponse<GetAllOrderDto> serviceResponse = new ServiceResponse<GetAllOrderDto>();
            try
            {
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
                //Verifica si hay ordenes disponibles del usuario
                var all_order = await this._serviceGenericOrderHelper._context
                .Include(c => c.CodeVerification)
                .Include(c => c.Gps)
                .Include(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.Product.Category)
                .Include(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                .Include(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Where(c => c.CodeVerification == code_identification)
                .OrderByDescending(c => c.OrderDate)
                .ToListAsync()
                .ConfigureAwait(false);
                if (!all_order.Any())
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotAnyOrder;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotAnyOrder);
                    return serviceResponse;
                }
                serviceResponse.Code = (int)GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = new GetAllOrderDto
                {
                    CodeVerification = code_identification.Code,
                    GetAllOrders = all_order.Select(c => new GetAllOrder 
                    {
                      Id = c.Id,
                      GetAllOrderDetails = c.OrderDetails.Select(x =>  new GetAllOrderDetail 
                      {
                        ProductCombinedId = x.ProductCombined.Id,
                        ProductId = x.ProductCombined.Product.Id,
                        Average = x.ProductCombined.Product.Average,
                        SupportAggregate = x.ProductCombined.Product.SupportAggregate,
                        Name = x.ProductCombined.Product.Name,
                        Price = x.ProductCombined.Price,
                        Description = x.ProductCombined.Product.Description,
                        IsAvailabe = x.ProductCombined.Product.IsAvailabe,
                        Quantity = x.ProductCombined.Quantity,
                        SubCategory = this._mapper.Map<GetSubCategoryDto>(x.ProductCombined.SubCategory),
                        Category = new Common.Dtos.Category.GetCategoryDto
                       {
                            Id = x.ProductCombined.Product.Category.Id,
                            Name = x.ProductCombined.Product.Category.Name,
                       },
                        CantAggregates = x.ProductCombined.CantAggregates.Select(z => new GetCantAggregateDto
                       {
                            Id = z.Aggregate.Id,
                            Name = z.Aggregate.Name,
                            Price = z.Price,
                            Quantity = z.Quantity,
                        }).ToList(),
                        DateCreated = x.DateCreated,
                      }).ToList(),
                      Address = c.Address,
                      AskForWho = c.AskForWho,
                      GetGps = this._mapper.Map<GetGps>(c.Gps),
                      PhoneNumber = c.PhoneNumber
                    }).ToList(),
                    DeliveryDate = DateTime.UtcNow,
                    OrderDate = DateTime.UtcNow,
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
