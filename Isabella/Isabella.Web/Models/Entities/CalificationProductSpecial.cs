namespace Isabella.Web.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Common.Extras;
    using Extras;
   
    /// <summary>
    /// Entidad que representa la calificación del Cliente acerca de un Producto
    /// </summary>
    public class CalificationProductSpecial : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }
      
        /// <summary>
        /// Producto 
        /// </summary>
        public ProductSpecial Product { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Fecha en la que el usuario dió la calificación del producto
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Calificación del cliente acerca del Producto(1-5 Estrellas)
        /// </summary>
        [Required(ErrorMessage = "Debe definir su calificación.")]
        public EnumCalification Calification { get; set; }

        /// <summary>
        /// Opinión del cliente acerca del Producto
        /// </summary>
        public string Opinion { get; set; }
    }
}
