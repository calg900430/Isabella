namespace Isabella.API.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Carro de compras Fast(Para pedidos informales, o sea usuarios que no desean ser clientes oficiales)
    /// </summary>
    public class CartShop : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Código de identificación.
        /// </summary>
        public User User {get; set; }

        /// <summary>
        /// ProductCarShop
        /// </summary>
        public ProductCombined ProductCombined { get; set; }

        /// <summary>
        /// Fecha en que se agrego el producto al carrito.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Precio Producto, el mismo se define si el usuario elige el producto con subcategoria o no.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceProductCombined
        {
            get
            {
                return ProductCombined.Price;
            }
        }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int QuantityProductCombined { get { return this.ProductCombined.Quantity; } }

        /// <summary>
        /// Precio total del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotalProductCombined
        {
            get
            {
                return (this.PriceProductCombined * (decimal)this.QuantityProductCombined);
            }
        }

        /// <summary>
        /// Cantidad Total de Agregados.
        /// </summary>
        public int QuantityTotalAggregate
        {
            get
            {
                if (!ProductCombined.CantAggregates.Any())
                    return 0;
                else
                    return this.ProductCombined.CantAggregates.Sum(x => x.Quantity);
            }
        }

        /// <summary>
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregates
        {
            get
            {
                if (!ProductCombined.CantAggregates.Any())
                return 0;
                else
                return this.ProductCombined.CantAggregates.Sum(x => x.PriceTotal);
            }
        }

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
    }
}
