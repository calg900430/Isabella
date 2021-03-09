namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateCarShopDto
    {
        /// <summary>
        /// Código de identificación.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el código de identificación.")]
        public Guid CodeIdentification { get; set; }

        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto combinado que está en su carrito.")]
        public int ProductCombinedId { get; set; }

        /// <summary>
        /// Asigna una subcategoria o la elimina.
        /// </summary>
        public int SubCategoryId { get; set; }
    }
}
