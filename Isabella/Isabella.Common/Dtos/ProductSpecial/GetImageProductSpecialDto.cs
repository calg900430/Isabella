namespace Isabella.Common.Dtos.ProductSpecial
{
    /// <summary>
    /// Imagen de un producto.
    /// </summary>
    public class GetImageProductSpecialDto
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
