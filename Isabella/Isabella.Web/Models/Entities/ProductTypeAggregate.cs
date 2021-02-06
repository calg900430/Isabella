namespace Isabella.Web.Models.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Agregados para productos especiales
    /// </summary>
    public class ProductTypeAggregate : Product
    {
        /// <summary>
        /// Categoria de los productos de agregados
        /// </summary>
        public CategoryProductTypeAggregate CategoryProductTypeAggregate { get; set; }
        /// <summary>
        /// Imagenes para los productos especiales. 
        /// </summary>
        public ICollection<ImageProductTypeAggregate>  ImageProductTypeAggregates { get; set; }
    }
}
