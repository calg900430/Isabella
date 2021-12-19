namespace Isabella.Common.Dtos.Aggregate
{
    using System.Collections.Generic;

    /// <summary>
    /// Agrega imagenes de un producto standard.
    /// </summary>
    public class AddImageAggregateDto
    {
        /// <summary>
        /// Id del producto.
        /// </summary>
        public int AggregateId { get; set; }

        /// <summary>
        /// Imagenes del producto.
        /// </summary>
        public byte[] Image {get; set;}
    }
}
