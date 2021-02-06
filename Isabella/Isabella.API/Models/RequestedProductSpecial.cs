namespace Isabella.API.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Extras;

    /// <summary>
    /// Cantidad de productos especiales que solicita el cliente
    /// </summary>
    public class RequestedProductSpecial : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto Especial
        /// </summary>
        public ProductSpecial ProductSpecial { get; set; }

        /// <summary>
        /// Queso Gouda
        /// </summary>
        public bool CheeseGouda { get; set; }

        /// <summary>
        /// Agregados para el producto 
        /// </summary>
        public ICollection<RequestedProductAggregate> RequestedProductAggregates { get; set; }

        /// <summary>
        /// Precio actual.
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
        public decimal PriceTotal 
        { 
            get 
            {
                //El pedido del producto espcial es con queso Gouda
                decimal price_gouda = 0;
                if(CheeseGouda)
                price_gouda = 35;
                //Obtiene el precio total de todos los agregos.
                var precio_total_agregados = RequestedProductAggregates.Sum(c => c.PriceTotal);
                return this.Price * (decimal)this.Quantity + precio_total_agregados + price_gouda;
            }
        }

    }
}
