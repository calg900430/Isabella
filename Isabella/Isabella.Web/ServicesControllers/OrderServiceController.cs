namespace Isabella.Web.ServicesControllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;

    using Common;
    using Common.Dtos.Order;
    using Common.RepositorysDtos;
    using Helpers;
    using Models.Entities;
    using Resources;
    using AutoMapper;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Isabella.Common.Dtos.SubCategorie;
    using Isabella.Common.Dtos.CarShop;
   
    using Helpers.RepositoryHelpers;
    using Hubs;
  
    /// <summary>
    /// Servicio para las ordenes
    /// </summary>
    public class OrderServiceController : IOrderRepositoryDto
    {
        private readonly ServiceGenericHelper<Order> _serviceGenericOrderHelper;
        private readonly ServiceGenericHelper<OrderDetail> _serviceGenericOrderDetailHelper;
        private readonly ServiceGenericHelper<CartShop> _serviceGenericCartShopHelper;
        private readonly IUserRepositoryHelper _userServiceHelper;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly DicctionaryConnectedHub _dicctionaryConnectedHubService;
        private readonly ServiceGenericHelper<NotificationPendients> _serviceGenericNotificationsPendientsHelper;
        private readonly ServiceGenericHelper<UserAdminsNotifications> _serviceGenericUserAdminsNotificationsHelper;
        private readonly IMapper _mapper;
        private static ITelegramBotClient _telegramBotClient;


        /// <summary>
        /// Claims del usuario.
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
      

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceGenericOrderHelper"></param>
        /// <param name="serviceGenericOrderDetailHelper"></param>
        /// <param name="serviceGenericCartShopHelper"></param>
        /// <param name="userServiceHelper"></param>
        /// <param name="hubContext"></param>
        /// <param name="dicctionaryConnectedHubService"></param>
        /// <param name="serviceGenericNotificationsPendientsHelper"></param>
        /// <param name="serviceGenericUserAdminsNotificationsHelper"></param>
        /// <param name="mapper"></param>
        public OrderServiceController(ServiceGenericHelper<Order> serviceGenericOrderHelper, 
        ServiceGenericHelper<OrderDetail> serviceGenericOrderDetailHelper, 
        ServiceGenericHelper<CartShop> serviceGenericCartShopHelper,
        IUserRepositoryHelper userServiceHelper,
        IHubContext<NotificationsHub> hubContext,
        DicctionaryConnectedHub dicctionaryConnectedHubService,
        ServiceGenericHelper<NotificationPendients> serviceGenericNotificationsPendientsHelper,
        ServiceGenericHelper<UserAdminsNotifications> serviceGenericUserAdminsNotificationsHelper,
        IMapper mapper)
        {
            this._serviceGenericOrderHelper = serviceGenericOrderHelper;
            this._serviceGenericOrderDetailHelper = serviceGenericOrderDetailHelper;
            this._serviceGenericCartShopHelper = serviceGenericCartShopHelper;
            this._userServiceHelper = userServiceHelper;
            this._hubContext = hubContext;
            this._dicctionaryConnectedHubService = dicctionaryConnectedHubService;
            this._serviceGenericNotificationsPendientsHelper = serviceGenericNotificationsPendientsHelper;
            this._serviceGenericUserAdminsNotificationsHelper = serviceGenericUserAdminsNotificationsHelper;
            this._mapper = mapper;
        }

        /// <summary>
        /// Confirma las ordenes de un usuario.
        /// </summary>
        /// <param name="confirmOrder"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ConfirmOrderAsync(ConfirmOrderDto confirmOrder)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Obtiene todos los productos del carrito del usuario
                var all_products_in_carshop = await this._serviceGenericCartShopHelper._context
                .Include(c => c.User)
                .Include(c => c.ProductCombined.Product.Category)
                .Include(c => c.ProductCombined.Product.SubCategories)
                .Include(c => c.ProductCombined.SubCategory.Product.Category)
                .Include(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Include(c => c.ProductCombined.CantAggregates)
                .Where(c => c.User == user)
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
                   PriceProductCombined = c.ProductCombined.Price,
                   PriceTotalOfAggregates = c.PriceTotalOfAggregates,
                   QuantityProductCombined = c.QuantityTotalProductCombined,
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
                    User = user,
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
                //Notifica al admin, que el usuario confirmó su pedido
                //await ConfirmOrderNotificationAsyncWithSignalR(order).ConfigureAwait(false);
                await ConfirmOrderNotificationAsyncWithTelegram(order).ConfigureAwait(false);
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
        /// Obtiene todas las ordenes del usuario.
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<GetAllOrderDto>> GetAllOrderAsync()
        {
            ServiceResponse<GetAllOrderDto> serviceResponse = new ServiceResponse<GetAllOrderDto>();
            try
            {
                var userName = ClaimsPrincipal.Identity.Name;
                if (userName == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.ErrorGetCredentialsUser;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.ErrorGetCredentialsUser);
                    return serviceResponse;
                }
                //Verifica si el usuario está registrado en la base de datos.
                var user = await this._userServiceHelper.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (user == null)
                {
                    serviceResponse.Code = (int)GetValueResourceFile.KeyResource.UserNotFound;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.UserNotFound);
                    return serviceResponse;
                }
                //Verifica si hay ordenes disponibles del usuario
                var all_order = await this._serviceGenericOrderHelper._context
                .Include(c => c.User)
                .Include(c => c.Gps)
                .Include(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.Product.Category)
                .Include(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product)
                .Include(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
                .Where(c => c.User == user)
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
                    GetUserDto = new Common.Dtos.Users.GetUserDto 
                    { 
                       Address = user.Address,
                       FirstName = user.FirstName,
                       Id = user.Id,
                       ImageUserProfile = user.ImageUserProfile,
                       LastName = user.LastName,
                       PhoneNumber = user.PhoneNumber,
                    } ,
                    GetAllOrders = all_order.Select(c => new GetOrderDto 
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
                        SubCategory = this._mapper.Map<GetSubCategorieDto>(x.ProductCombined.SubCategory),
                        Category = new Common.Dtos.Categorie.GetCategorieDto
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
        /// Notifica a los usuarios admin a través de SignalR que un usuario ha realizado un pedido.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task ConfirmOrderNotificationAsyncWithSignalR(Order order)
        {
            //Obtiene los usuarios admins a los que se le debe notificar.
            var users_admins_notifications = await this._serviceGenericUserAdminsNotificationsHelper
            .GetLoadAsync(c => c.User).ConfigureAwait(false);
            if (users_admins_notifications == null)
            return;
            foreach(User user in users_admins_notifications.Select(c => c.User).ToList())
            {
                //Verifica q el usuario admin este conectado
                var user_connected = this._dicctionaryConnectedHubService.VerifyIsUserConnected(user.UserName);
                if (user_connected)
                {
                    //Obtiene los dispositivos del usuario que están conectados para enviarle la notificación
                    var devices_connected = this._dicctionaryConnectedHubService.GetAllDeviceConnectedOfUser(user.UserName);
                    if(devices_connected == null)
                    {
                        var notification = new NotificationPendients
                        {
                            Order = order,
                            UserAdmin = user,
                        };
                        await this._serviceGenericNotificationsPendientsHelper.AddEntityAsync(notification).ConfigureAwait(false);
                        await this._serviceGenericNotificationsPendientsHelper.SaveChangesBDAsync().ConfigureAwait(false);
                        return;
                    }
                    //Mapea de Order a GetAllOrderDto
                    var get_all_order = new GetAllOrderDto
                    {
                        DeliveryDate = DateTime.UtcNow,
                        OrderDate = DateTime.UtcNow,
                        GetUserDto = new Common.Dtos.Users.GetUserDto
                        {
                            Address = user.Address,
                            FirstName = user.FirstName,
                            Id = user.Id,
                            ImageUserProfile = user.ImageUserProfile,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber,
                        },
                        GetAllOrders = new List<GetOrderDto>
                        {
                            new GetOrderDto
                            {
                                Id = order.Id,
                                GetAllOrderDetails = order.OrderDetails.Select(x =>  new GetAllOrderDetail
                                {
                                   ProductCombinedId = x.ProductCombined.Id,
                                   ProductId = x.ProductCombined.Product.Id,
                                   Average = x.ProductCombined.Product.Average,
                                   SupportAggregate = x.ProductCombined.Product.SupportAggregate,
                                   Name = x.ProductCombined.Product.Name,
                                   Price = x.ProductCombined.Product.Price,
                                   Description = x.ProductCombined.Product.Description,
                                   IsAvailabe = x.ProductCombined.Product.IsAvailabe,
                                   Quantity = x.ProductCombined.Quantity,
                                   SubCategory = this._mapper.Map<GetSubCategorieDto>(x.ProductCombined.SubCategory),
                                   Category = new Common.Dtos.Categorie.GetCategorieDto
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
                                Address = order.Address,
                                AskForWho = order.AskForWho,
                                PhoneNumber = order.PhoneNumber,
                                GetGps = this._mapper.Map<GetGps>(order.Gps),
                            }      
                        },
                        
                    };
                    //Serializa el objeto get_all_order
                    var order_serialize = JsonConvert.SerializeObject(get_all_order);
                    await this._hubContext.Clients.Clients(devices_connected.ToList())
                   .SendAsync("ConfirmOrder", "Se ha solicitado un nuevo pedido.", $"{order_serialize.ToString()}")
                   .ConfigureAwait(false);
                }
                //El usuario admin no está conectado.
                else
                {
                    //Guarda las notificaciones en la base de datos hasta q el usuario admin se conecte.
                    var notification = new NotificationPendients
                    {
                        Order = order,
                        UserAdmin = user,
                    };
                    await this._serviceGenericNotificationsPendientsHelper.AddEntityAsync(notification).ConfigureAwait(false);
                    await this._serviceGenericNotificationsPendientsHelper.SaveChangesBDAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Notifica a los usuarios admin a través de Telegram que un usuario ha realizado un pedido.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task ConfirmOrderNotificationAsyncWithTelegram(Order order)
        {
            //Crea la conexión con la api de telegram
            //y el Bot Comidaalacasa a través de su Token
            _telegramBotClient =
            new TelegramBotClient("1881001855:AAEo8Fw0wLjnTQTijfEo-" +
            "CTVQn3RMuO2Wuw");

            var a = await _telegramBotClient.GetUpdatesAsync().ConfigureAwait(false);

            //Obtiene los usarios admins que deben recibir las notificaciones
            var users_admins = await _serviceGenericUserAdminsNotificationsHelper
            .GetLoadAsync().ConfigureAwait(false);

            //Arma los detalles de la orden //1382606262
            string orderdetail = "";
            int i = 1;
            foreach (OrderDetail orderDetail in order.OrderDetails)
            {
                if(orderDetail.ProductCombined.SubCategory == null && 
                orderDetail.ProductCombined.CantAggregates.Count <= 0)
                {
                    orderdetail += $"--------------------------------------------\n" +
                                   $"#{i}: {orderDetail.ProductCombined.Product.Name}\n" +
                                   $"  Cantidad = {orderDetail.ProductCombined.Quantity}\n" +
                                   $"  SubTotal = {orderDetail.PriceTotal}\n" +
                                   $"---------------------------------------------\n";
                }
                if(orderDetail.ProductCombined.SubCategory != null &&
                orderDetail.ProductCombined.CantAggregates.Count <= 0)
                {
                    orderdetail += $"--------------------------------------------\n" +
                                   $"#{i}: {orderDetail.ProductCombined.Product.Name}\n" +
                                   $"  Cantidad: {orderDetail.ProductCombined.Quantity}\n" +
                                   $"  Tipo: {orderDetail.ProductCombined.SubCategory.Name}\n" +
                                   $"  SubTotal = {orderDetail.PriceTotal}\n" +
                                   $"---------------------------------------------\n";
                }
                if (orderDetail.ProductCombined.SubCategory == null &&
                orderDetail.ProductCombined.CantAggregates.Count > 0)
                {
                    string cantaggregate = "";
                    int k = 1;
                    foreach (CantAggregate cantAggregate in orderDetail.ProductCombined.CantAggregates)
                    {
                        if(k != orderDetail.ProductCombined.CantAggregates.Count)
                        cantaggregate += $"+{cantAggregate.Quantity}:{cantAggregate.Aggregate.Name}\n" + "   ";
                        if(k == orderDetail.ProductCombined.CantAggregates.Count)
                        cantaggregate += $"+{cantAggregate.Quantity}:{cantAggregate.Aggregate.Name}\n";
                        k++;
                    }
                    orderdetail += $"---------------------------------------------\n" +
                              $"#{i}: {orderDetail.ProductCombined.Product.Name}\n" +
                              $" Cantidad:{orderDetail.ProductCombined.Quantity}\n" +
                              $"  Agregos = {orderDetail.ProductCombined.CantAggregates.Count}\n" +
                              $"   {cantaggregate}" +
                              $"  SubTotal:{orderDetail.PriceTotal}\n" +
                              $"---------------------------------------------\n";
                }
                if (orderDetail.ProductCombined.SubCategory != null &&
                orderDetail.ProductCombined.CantAggregates.Count > 0)
                {
                    string cantaggregate = "";
                    int k = 1;
                    foreach (CantAggregate cantAggregate in orderDetail.ProductCombined.CantAggregates)
                    {
                        if (k != orderDetail.ProductCombined.CantAggregates.Count)
                        cantaggregate += $"+{cantAggregate.Quantity}:{cantAggregate.Aggregate.Name}\n" + "   ";
                        if (k == orderDetail.ProductCombined.CantAggregates.Count)
                        cantaggregate += $"+{cantAggregate.Quantity}:{cantAggregate.Aggregate.Name}\n";
                        k++;
                    }
                    orderdetail += $"---------------------------------------------\n" +
                             $"#{i}: {orderDetail.ProductCombined.Product.Name}\n" +
                             $"  Tipo: {orderDetail.ProductCombined.SubCategory.Name}\n" +
                             $"  Cantidad:{orderDetail.ProductCombined.Quantity}\n" +
                             $"  Agregos = {orderDetail.ProductCombined.CantAggregates.Count}\n" +
                             $"   {cantaggregate}" +
                             $"  SubTotal:{orderDetail.PriceTotal}\n" +
                             $"---------------------------------------------\n";
                }
                i++;
            }

            //Envia la confirmación de las ordenes a los usuarios admins autorizados a recibir notificaciones
            foreach (UserAdminsNotifications userAdmins in users_admins)
            {
              await _telegramBotClient
              .SendTextMessageAsync(userAdmins.UserTelegramChatId,
              $"=======Nueva Orden=======\n" +
              $"Dirección: {order.Address}\n" +
              $"Preguntar por: {order.AskForWho}\n" +
              $"Telefono: {order.PhoneNumber}\n" +
              $"Cantidad Total de Productos:{order.QuantityTotalProductCombined}\n" +
              $" {orderdetail}" +
              $"Precio Total de Orden:{order.PriceTotal}\n" +
              $"===============================")
              .ConfigureAwait(false);
            }                                        
        }
    }
}
