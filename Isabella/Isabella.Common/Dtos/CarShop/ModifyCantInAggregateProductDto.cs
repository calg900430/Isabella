namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ModifyCantInAggregateProductDto
    {
        /// <summary>
        /// ProductoCombinado.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto combinado que está en su carrito.")]
        public int ProductCombinedId { get; set; }

        /// <summary>
        /// ProductoCombinado.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del agregado que desea añadir a su producto.")]
        public int AggregateId { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la nueva cantidad del agregado.")]
        public int Quantity { get; set; }
    }
}
