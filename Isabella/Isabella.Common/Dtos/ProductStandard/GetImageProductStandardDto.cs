namespace Isabella.Common.Dtos.ProductStandard
{
    /// <summary>
    /// Imagen de un producto.
    /// </summary>
    public class GetImageProductStandardDto
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
