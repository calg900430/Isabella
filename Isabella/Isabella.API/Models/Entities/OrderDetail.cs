namespace Isabella.API.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Detalles de un pedido de tipo Fast
    /// </summary>
    public class OrderDetail : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ProductCarShop
        /// </summary>
        public ProductCombined ProductCombined { get; set; }

        /// <summary>
        /// Precio Producto, el mismo se define si el usuario elige el producto con subcategoria o no.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceProductCombined { get; set; }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int QuantityProductCombined { get; set; }

        /// <summary>
        /// Precio total del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotalProductCombined { get; set; }

        /// <summary>
        /// Cantidad Total de Agregados.
        /// </summary>
        public int QuantityTotalAggregate { get; set; }

        /// <summary>
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregates { get; set; }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotal
        {
            get
            {
                return PriceTotalProductCombined + PriceTotalOfAggregates;
            }
        }

        /// <summary>
        /// Fecha en que se agrego el producto al carrito.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }
    }
}
