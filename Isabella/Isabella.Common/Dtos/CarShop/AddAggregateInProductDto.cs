namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class AddAggregateInProductDto
    {
        /// <summary>
        /// Código de identificación.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el código de identificación.")]
        public Guid CodeIdentification { get; set; }

        /// <summary>
        /// ProductoStandard.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el id del producto combinado.")]
        public int ProductCombinedId { get; set; }

        /// <summary>
        /// Agregado.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el agregado.")]
        public int AggregateId { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad del agregado.")]
        public int Quantity { get; set; }
    }
}
