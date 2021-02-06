namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Isabella.Common.Dtos.ProductStandard;

    public class AddProductStandardToCarShopDto
    {
        /// <summary>
        /// Código de identificación.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el código de identificación.")]
        public Guid Verification { get; set; }

        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto.")]
        public int ProductStandardId { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad de productos.")]
        public int Quantity { get; set; }
    }
}
