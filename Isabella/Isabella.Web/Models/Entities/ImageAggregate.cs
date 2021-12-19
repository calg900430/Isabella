namespace Isabella.Web.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Models;

    /// <summary>
    /// Entidad que representa las imagenes de los Productos
    /// </summary>
    public class ImageAggregate : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto.
        /// </summary>
        public Aggregate Aggregate { get; set; }

        /// <summary>
        /// Imagen del producto.
        /// </summary>
        public byte[] Image { get; set; }
    }
}
