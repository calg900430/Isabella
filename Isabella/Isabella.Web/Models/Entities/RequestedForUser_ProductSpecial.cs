namespace Isabella.Web.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Isabella.Web.Extras;

    /// <summary>
    /// Cantidad de productos especiales que solicita el cliente
    /// </summary>
    public class RequestedForUser_ProductSpecial : IModel
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
        /// Producto Especial
        /// </summary>
        public ProductSpecial ProductSpecial { get; set; }

        /// <summary>
        /// Tipo de queso
        /// </summary>
        public RequestedForUser_ProductTypeCheese RequestedForUser_ProductTypeCheese { get; set; }

        /// <summary>
        /// Agregados para el producto 
        /// </summary>
        public ICollection<RequestedForUser_ProductTypeAggregate> RequestedForUser_ProductTypeAggregates { get; set; }

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
