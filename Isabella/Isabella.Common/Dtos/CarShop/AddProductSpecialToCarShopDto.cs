namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddProductSpecialToCarShopDto
    {
        /// <summary>
        /// Código de identificación.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el código de identificación.")]
        public Guid Verification { get; set; }

        /// <summary>
        /// ProductoSpecial.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto.")]
        public int ProductSpecialId { get; set; }

        /// <summary>
        /// Con Queso Gouda
        /// </summary>
        public bool CheeseGouda { get; set; }

        /// <summary>
        /// Cantidad de productos de agregos.
        /// </summary>
        public List<CantProductAggregate> CantProductAggregates { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad de productos.")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Cantidad de productos de agrego
    /// </summary>
    public class CantProductAggregate
    {
        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto.")]
        public int ProductAggregateId { get; set; }

        /// <summary>
        /// Cantidad que desea.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad de productos.")]
        public int Quantity { get; set; }
    }
}
