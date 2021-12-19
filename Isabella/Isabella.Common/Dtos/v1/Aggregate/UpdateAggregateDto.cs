namespace Isabella.Common.Dtos.Aggregate
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Categorie;

    public class UpdateAggregateDto
    {
        /// <summary>
        /// Id del Producto.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el Id del producto.")]
        public int AggregateId { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el nombre del producto.")]
        public string Name { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool? IsAvailabe { get; set; } = true;

        /// <summary>
        /// Cantidad disponible en el Stock
        /// </summary>
        public int? Stock { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? Price { get; set; }

        /// <summary>
        /// Descripción del Producto.
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para dar una descripción del producto.")]
        public string Description { get; set; }
    }
}
