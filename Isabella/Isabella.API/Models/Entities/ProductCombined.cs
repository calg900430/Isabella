namespace Isabella.API.Models.Entities
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
        /// Precio Producto, el mismo se define si el usuario elige el producto con subcategoria o no.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price 
        {
           get
           { 
              if(this.SubCategory == null)
              return this.Product.Price;
              else
              return this.SubCategory.Price;
           }
        }

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
               decimal price_all_aggregate = 0;
               if(CantAggregates.Any())
               price_all_aggregate = this.CantAggregates.Sum(c => c.PriceTotal);
               return (this.Price * (decimal)this.Quantity) + price_all_aggregate;
            }
        }
    }
}
