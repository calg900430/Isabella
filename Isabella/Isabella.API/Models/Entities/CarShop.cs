namespace Isabella.API.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Carro de compras Fast(Para pedidos informales, o sea usuarios que no desean ser clientes oficiales)
    /// </summary>
    public class CarShop : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Código de identificación.
        /// </summary>
        public CodeIdentification CodeIdentification {get; set; }

        /// <summary>
        /// Producto Standard
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Cantidad de agregos.
        /// </summary>
        public ICollection<CantAggregate> CantAggregates { get; set; }

        /// <summary>
        /// Queso Gouda.
        /// </summary>
        public bool? CheeseGouda { get; set; }

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
        public decimal PriceTotal
        { 
          get 
          {
             //Verifica si el producto lleva queso gouda
             decimal price_cheese_gouda = 0;
             if(this.CheeseGouda == true)
             price_cheese_gouda = 35;
             else
             price_cheese_gouda = 0;
             //Suma los precios total de todos los agregados solicitados
             decimal price_aggregates = 0;
             if(this.CantAggregates != null || this.CantAggregates.Count > 0)
             price_aggregates = this.CantAggregates.Sum(c => c.PriceTotal);
             else
             price_aggregates = 0;
             //Obtirne el precio total del posible pedido
             return (this.Price * (decimal)this.Quantity) + price_cheese_gouda + price_aggregates; 
          }
        }
    }
}
