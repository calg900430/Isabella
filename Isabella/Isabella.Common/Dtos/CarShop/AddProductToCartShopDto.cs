namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Isabella.Common.Dtos.Product;

    public class AddProductToCartShopDto
    {
        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto.")]
        public int ProductId { get; set; }

        /// <summary>
        /// SubCategoría.
        /// </summary>
        public int? SubCategoryId{ get; set; }

        /// <summary>
        /// Cantidad de agregos que desea incluirle al producto.
        /// </summary>
        public Dictionary<string, int> CantAggregates { get; set; }
        
        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad de productos.")]
        public int Quantity { get; set; }
    }
}
