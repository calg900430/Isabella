namespace Isabella.Web.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    using Data;
    using Models.Entities;
    using Common.Dtos.Order;
    using Common.Dtos.SubCategorie;
    using AutoMapper;
    using Common.Dtos.CarShop;

    /// <summary>
    /// Hub para las notificaciones que usa Isabellsa 
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationsHub : Hub
    {
        private readonly DicctionaryConnectedHub _dicctionaryConnectedHubService;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicctionaryConnectedHubService"></param>
        /// <param name="dataContext"></param>
        /// <param name="mapper"></param>
        public NotificationsHub(DicctionaryConnectedHub dicctionaryConnectedHubService,DataContext dataContext, IMapper mapper)
        {
            this._dicctionaryConnectedHubService = dicctionaryConnectedHubService;
            this._dataContext = dataContext;
            this._mapper = mapper;
        }
 
        /// <summary>
        /// Evento OnConnected, se ejecuta cada vez que un usuario se conecta.
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            string userName = Context.User.Identity.Name;
            if (userName == null)
            return;
            //Agrega un usuario al diccionario de conexiones
            _dicctionaryConnectedHubService.AddNewDevice(userName, Context.ConnectionId);
            //Verifica si el usuario tiene notificaciones pendiente.
            var notifications_pendients = await this._dataContext.NotificationPendients
            .Include(c => c.Order).ThenInclude(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.Product.Categorie)
            .Include(c => c.Order).ThenInclude(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.Product.SubCategories).ThenInclude(c => c.Product.Categorie)
            .Include(c => c.Order).ThenInclude(c => c.OrderDetails).ThenInclude(c => c.ProductCombined.CantAggregates).ThenInclude(c => c.Aggregate)
            .Include(c => c.Order).ThenInclude(c => c.User)
            .Include(c => c.UserAdmin)
            .Where(c => c.UserAdmin.UserName == userName)
            .ToListAsync()
            .ConfigureAwait(false);
            //Es un usuario que puede recibir notificaciones.
            if (notifications_pendients.Any())
            {
                //Obtiene los dispositivos del usuario que están conectados para enviarle la notificación
                var devices_connected = this._dicctionaryConnectedHubService
                .GetAllDeviceConnectedOfUser(notifications_pendients.Select(c => c.UserAdmin.UserName).FirstOrDefault());
                foreach (NotificationPendients notificationPendients in notifications_pendients)
                {
                    var get_all_order = new GetAllOrderDto
                    {
                        DeliveryDate = DateTime.UtcNow,
                        OrderDate = DateTime.UtcNow,
                        GetUserDto = new Common.Dtos.Users.GetUserDto
                        {
                            Address = notificationPendients.Order.User.Address,
                            FirstName = notificationPendients.Order.User.FirstName,
                            Id = notificationPendients.Order.User.Id,
                            ImageUserProfile = notificationPendients.Order.User.ImageUserProfile,
                            LastName = notificationPendients.Order.User.LastName,
                            PhoneNumber = notificationPendients.Order.User.PhoneNumber,
                        },
                        GetAllOrders = new List<GetOrderDto>
                            {
                              new GetOrderDto
                              {
                                Id = notificationPendients.Order.Id,
                                GetAllOrderDetails = notificationPendients.Order.OrderDetails.Select(x =>  new GetAllOrderDetail
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
                                      Id = x.ProductCombined.Product.Categorie.Id,
                                      Name = x.ProductCombined.Product.Categorie.Name,
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
                                Address = notificationPendients.Order.Address,
                                AskForWho = notificationPendients.Order.AskForWho,
                                PhoneNumber = notificationPendients.Order.PhoneNumber,
                                GetGps = this._mapper.Map<GetGps>(notificationPendients.Order.Gps),
                              }
                            },
                    };
                    //Serializa el objeto get_all_order
                    var order_serialize = JsonConvert.SerializeObject(get_all_order);
                    await Clients.Clients(devices_connected.ToList())
                   .SendAsync("ConfirmOrder", "Se ha solicitado un nuevo pedido.", $"{order_serialize.ToString()}")
                   .ConfigureAwait(false);
                }
                //Elimina las notificaciones.
                this._dataContext.NotificationPendients.RemoveRange(notifications_pendients);
                await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            }
            await base.OnConnectedAsync().ConfigureAwait(false);
        }
   
        /// <summary>
        /// Evento OnDisconnectedAsync, se ejecuta cada vez que 
        /// un usuario se desconecta.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //Elimina un usuario del diccionario de conexiones
            string name = Context.User.Identity.Name;
            _dicctionaryConnectedHubService.RemoveDevice(name, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }
    }
}
