namespace Isabella.API.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Models;
    using Hubs.Services;

    /// <summary>
    /// Hub para las notificaciones que usa Isabellsa 
    /// </summary>
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationsHub : Hub
    {
        private readonly DicctionaryConnectedHub _dicctionaryConnectedHubService;
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public NotificationsHub(DicctionaryConnectedHub dicctionaryConnectedHubService, DataContext dataContext)
        {
            this._dicctionaryConnectedHubService = dicctionaryConnectedHubService;
            this._dataContext = dataContext;
        }


        /// <summary>
        /// Evento OnConnected, se ejecuta cada vez que un usuario se conecta.
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            //Agrega un usuario al diccionario de conexiones
            string name = Context.User.Identity.Name;
            _dicctionaryConnectedHubService.AddNewDevice(name, Context.ConnectionId);
            await base.OnConnectedAsync();
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
