namespace Isabella.Web.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Isabella.Web.Extras;

    /// <summary>
    /// Agregados
    /// </summary>
    public class RequestedForUser_ProductTypeAggregate : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Producto de agrego
        /// </summary>
        public ProductTypeAggregate ProductTypeAggregate { get; set; }

        /// <summary>
        /// Precio actual del Producto Standard
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad de Productos Standards
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ProductSpecial_Value { get { return this.Price * (decimal)this.Quantity; } }
    }
}
