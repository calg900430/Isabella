namespace Isabella.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Extras;

    /// <summary>
    /// Agregados
    /// </summary>
    public class CarShopProductAggregate : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Carrito de Compras
        /// </summary>
        public CarShopProductSpecial CarShopProductSpecial { get; set; }

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
