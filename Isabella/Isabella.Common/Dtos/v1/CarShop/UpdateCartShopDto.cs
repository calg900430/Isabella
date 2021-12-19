namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class UpdateCartShopDto
    {
       
        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto combinado que está en su carrito.")]
        public int ProductCombinedId { get; set; }

        /// <summary>
        /// Cantidad de agregos que desea incluirle al producto.
        /// </summary>
        public Dictionary<string, int> CantAggregates { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        public int? QuantityProduct { get; set; }

        /// <summary>
        /// Asigna una subcategoria o la elimina.
        /// </summary>
        public int? SubCategoryId { get; set; }
    }
}
