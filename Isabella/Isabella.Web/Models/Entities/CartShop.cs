namespace Isabella.Web.Models.Entities
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
        /// 
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
        /// Cantidad de Productos.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int QuantityTotalProductCombined { get { return this.ProductCombined.Quantity; } }

        /// <summary>
        /// Cantidad Total de Agregados.
        /// </summary>
        public int QuantityTotalAggregate
        {
            get
            {
                if(ProductCombined.CantAggregates != null)
                {
                    if (!ProductCombined.CantAggregates.Any())
                    return 0;
                    else
                    return this.ProductCombined.CantAggregates.Sum(x => x.Quantity);
                }
                else
                return 0;
            }
        }

        /// <summary>
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregates
        {
            get
            {
                if (ProductCombined.CantAggregates != null)
                {
                    if (!ProductCombined.CantAggregates.Any())
                    return 0;
                    else
                    return this.ProductCombined.CantAggregates.Sum(x => x.PriceTotal);
                }
                else
                return 0;

            }
        }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotal
        {
            get
            {
                if (this.ProductCombined.SubCategory != null)
                {
                    if(this.ProductCombined.CantAggregates != null)
                    {
                        if (this.ProductCombined.CantAggregates.Any())
                        return (this.ProductCombined.SubCategory.Price +
                        this.ProductCombined.CantAggregates.Sum(c => c.PriceTotal))
                        * (this.ProductCombined.Quantity);
                        else
                        return this.ProductCombined.SubCategory.Price * this.ProductCombined.Quantity;
                    }
                    else
                    return this.ProductCombined.SubCategory.Price * this.ProductCombined.Quantity;
                }
                else
                {
                    if (this.ProductCombined.CantAggregates != null)
                    {
                        if (this.ProductCombined.CantAggregates.Any())
                        return (this.ProductCombined.Product.Price +
                        this.ProductCombined.CantAggregates.Sum(c => c.PriceTotal))
                        * (this.ProductCombined.Quantity);
                        else
                        return this.ProductCombined.Product.Price * this.ProductCombined.Quantity;
                    }
                    else
                    return this.ProductCombined.Product.Price * this.ProductCombined.Quantity;
                }
            }
        }
    }
}
