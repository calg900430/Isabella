namespace Isabella.Web.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Notificaciones Pendientes para el admin.
    /// </summary>
    public class NotificationPendients : IEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Usuario admin a los que se le debe notificar.
        /// </summary>
        public User UserAdmin { get; set; }

        /// <summary>
        /// Ordenes.
        /// </summary>
        public Order Order { get; set; }
    }
}
