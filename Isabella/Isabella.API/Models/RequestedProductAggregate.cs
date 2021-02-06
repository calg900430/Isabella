namespace Isabella.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Extras;

    /// <summary>
    /// Agregados
    /// </summary>
    public class RequestedProductAggregate : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Pedido de producto especial que solicita productos de agregos.
        /// </summary>
        public RequestedProductSpecial RequestedProductSpecial { get; set; }

        /// <summary>
        /// Producto de agrego
        /// </summary>
        public ProductAggregate ProductAggregate { get; set; }

        /// <summary>
        /// Precio actual del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal { get { return this.Price * (decimal)this.Quantity; } }
    }
}
