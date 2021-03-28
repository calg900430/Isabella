namespace Isabella.Common.RepositorysDtos
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Funciones del cliente que puede ejecutar el server a traves de SignalR 
    /// </summary>
    public interface IClientHubRepository
    {
        /// <summary>
        /// Informa al usuario admin que un usuario ha realizado un pedido.
        /// Si el usuario admin no está online, se guarda la notificación hasta q este online.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OrderId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ConfirmOrder(string order, string message);
    }
}
