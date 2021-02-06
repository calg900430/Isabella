namespace Isabella.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Isabella.Common.Extras;
   
    /// <summary>
    /// Calificación de los productos Standard
    /// </summary>
    public class CalificationProductStandard
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto 
        /// </summary>
        public ProductStandard Product { get; set; }

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
        public EnumCalification Calification { get; set; }

        /// <summary>
        /// Opinión del cliente acerca del Producto
        /// </summary>
        public string Opinion { get; set; }
    }
}
