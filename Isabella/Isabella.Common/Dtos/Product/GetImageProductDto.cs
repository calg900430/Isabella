namespace Isabella.Common.Dtos.Product
{
    /// <summary>
    /// Imagen de un producto.
    /// </summary>
    public class GetImageProductDto
    {
        /// <summary>
        /// Id del producto
        /// </summary>
        public int ProductId { get; set; }

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
