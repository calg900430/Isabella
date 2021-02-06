namespace Isabella.API.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Agregados para productos especiales
    /// </summary>
    public class ProductAggregate : Product
    {
        /// <summary>
        /// Categoria de los productos de agregados
        /// </summary>
        public CategoryProductAggregate CategoryProductAggregate { get; set; }

        /// <summary>
        /// Imagenes para los productos especiales. 
        /// </summary>
        public ICollection<ImageProductAggregate> ImageProductAggregates { get; set; }
    }
}
