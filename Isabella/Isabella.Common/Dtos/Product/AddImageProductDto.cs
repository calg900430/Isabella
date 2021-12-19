namespace Isabella.Common.Dtos.Product
{
    using System.Collections.Generic;

    /// <summary>
    /// Agrega imagenes de un producto standard.
    /// </summary>
    public class AddImageProductDto
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
