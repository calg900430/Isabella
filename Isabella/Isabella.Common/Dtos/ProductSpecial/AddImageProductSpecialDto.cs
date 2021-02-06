namespace Isabella.Common.Dtos.ProductSpecial
{
    using System.Collections.Generic;

    /// <summary>
    /// Agrega imagenes de un producto standard.
    /// </summary>
    public class AddImageProductSpecialDto
    {
        /// <summary>
        /// Id del producto.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Imagenes del producto.
        /// </summary>
        public byte[] Image {get; set;}
    }
}
