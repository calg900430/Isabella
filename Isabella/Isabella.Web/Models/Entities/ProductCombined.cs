namespace Isabella.Web.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// ProductCarShop
    /// </summary>
    public class ProductCombined : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// SubCategoria del Producto.
        /// </summary>
        public SubCategory SubCategory { get; set; }

        /// <summary>
        /// Cantidad de agregos.
        /// </summary>
        public ICollection<CantAggregate> CantAggregates { get; set; }

        /// <summary>
        /// Precio Producto.
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
        public decimal PriceTotal
        {
            get
            {
               return (this.Price * (decimal)this.Quantity);
            }
        }
    }
}
