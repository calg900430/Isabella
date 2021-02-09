namespace Isabella.API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Extras;

    /// <summary>
    /// Agregados para productos especiales
    /// </summary>
    public class ProductAggregate : Product
    {
        /// <summary>
        /// Imagenes para los productos especiales. 
        /// </summary>
        public ICollection<ImageProductAggregate> ImageProductAggregates { get; set; }
    }
}
