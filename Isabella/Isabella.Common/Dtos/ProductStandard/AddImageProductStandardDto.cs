namespace Isabella.Common.Dtos.ProductStandard
{
    using System.Collections.Generic;

    /// <summary>
    /// Agrega imagenes de un producto standard.
    /// </summary>
    public class AddImageProductStandardDto
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
