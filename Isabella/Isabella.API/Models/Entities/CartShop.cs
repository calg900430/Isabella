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
        public CodeIdentification CodeIdentification {get; set; }

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
        /// Compara un CarShop
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool MyEqualTo(CartShop other)
        {
            if(this.CodeIdentification != other.CodeIdentification)
            return false;
            if (this.ProductCombined.Product.Id != other.ProductCombined.Product.Id)
            return false;
            if (this.ProductCombined.SubCategory.Id != other.ProductCombined.SubCategory.Id)
            return false;
            return this.ProductCombined.CantAggregates.Select(c => c.Aggregate.Id).ToList() 
            .SequenceEqual(other.ProductCombined.CantAggregates.Select(c => c.Aggregate.Id));
        }
    }
}
