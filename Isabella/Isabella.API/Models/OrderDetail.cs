namespace Isabella.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Detalles de un pedido de tipo Fast
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Orden
        /// </summary>
        public Order Order { get; set; }

        
        /// <summary>
        /// Fecha en que se agregó el producto al carrito de compras
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Fecha en que se actualizo el producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateUpdate { get; set; }
    }
}
