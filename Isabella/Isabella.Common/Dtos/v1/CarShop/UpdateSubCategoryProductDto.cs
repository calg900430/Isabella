namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateSubCategoryProductDto
    {
        
        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto combinado que está en su carrito.")]
        public int ProductCombinedId { get; set; }

        /// <summary>
        /// Asigna una subcategoria.
        /// </summary>
        public int? SubCategoryId { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad deseada.")]
        public int? QuantityProduct { get; set; }
    }
}
