namespace Isabella.Web.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Usuarios admins que deben recibir las notificaciones.
    /// </summary>
    public class UserAdminsNotifications : IEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Usuario admin a los que se le debe notificar.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Id de Chat de Telegram
        /// </summary>
        public int UserTelegramChatId { get; set; }
    }
}
