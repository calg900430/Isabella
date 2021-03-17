namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Modifica la cantidad de un producto del carrito
    /// </summary>
    public class ModifyQuantityProductDto
    {
        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto combinado que está en su carrito.")]
        public int ProductCombinedId { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad deseada.")]
        public int Quantity { get; set; }
    }
}
