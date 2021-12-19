namespace Isabella.Common.Dtos.Aggregate
{
    /// <summary>
    /// Imagen de un producto.
    /// </summary>
    public class GetImageAggregateDto
    {
        /// <summary>
        /// Id del producto
        /// </summary>
        public int AggregateId { get; set; }

        /// <summary>
        /// Id de imagen
        /// </summary>
        public int ImageId { get; set; }

        /// <summary>
        /// Imagen
        /// </summary>
        public byte[] Image { get; set; }
    }
}
