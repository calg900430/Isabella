namespace Isabella.Web.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Common;
   
    /// <summary>
    /// Calificación de los productos Standard
    /// </summary>
    public class CalificationProduct : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Producto 
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Fecha en la que el usuario dió la calificación del producto
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Calificación del cliente acerca del Producto(1-5 Estrellas)
        /// </summary>
        public CommonConstants.EnumCalification Calification { get; set; }

        /// <summary>
        /// Opinión del cliente acerca del Producto
        /// </summary>
        public string Opinion { get; set; }
    }
}
