namespace Isabella.Common.Dtos.Product
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Category;

    public class UpdateProductDto
    {
        /// <summary>
        /// Id del Producto.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el Id del producto.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el nombre del producto.")]
        public string Name { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool? IsAvailabe { get; set; }

        /// <summary>
        /// Indica si el producto se le puede incluir agregados.
        /// </summary>
        public bool? SupportAggregate { get; set; }

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
