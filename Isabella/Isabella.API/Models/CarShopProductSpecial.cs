namespace Isabella.API.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
   
    /// <summary>
    /// Carro de compras Fast(Para pedidos informales, o sea usuarios que no desean ser clientes oficiales)
    /// </summary>
    public class CarShopProductSpecial
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
        public ICollection<CarShopProductAggregate> CarShopProductAggregates { get; set; }

        /// <summary>
        /// Precio actual.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        /// <summary>
        /// Precio Total de los productos de agregado.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotalProductAggregate
        {
            get
            {
                decimal precio_total_agregados = 0;
                if (CarShopProductAggregates != null)
                precio_total_agregados = CarShopProductAggregates.Sum(c => c.PriceTotal);
                return precio_total_agregados;
            }
        }

        /// <summary>
        /// Cantidad Total de los productos de agregado.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int QuantityTotalProductAggregate 
        { 
            get
            {
                int quantity = 0;
                if (CarShopProductAggregates != null)
                quantity = CarShopProductAggregates.Sum(c => c.Quantity);
                return quantity;
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
                //El pedido del producto espcial es con queso Gouda
                decimal price_gouda = 0;
                if (CheeseGouda)
                price_gouda = 35;
                return (this.Price * (decimal) this.Quantity) + this.PriceTotalProductAggregate + price_gouda;
            }
        }
    }
}
