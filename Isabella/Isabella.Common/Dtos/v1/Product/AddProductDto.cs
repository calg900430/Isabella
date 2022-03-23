namespace Isabella.Common.Dtos.Product
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Agrega un nuevo producto standard.
    /// </summary>
    public class AddProductDto
    {
        /// <summary>
        /// Categoria del producto.
        /// </summary>
        [Required(ErrorMessage = "Es necesario que agregue la categoria del producto.")]
        public int CategorieId { get; set; }

        /// <summary>
        /// Nombre 
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el nombre del producto.")]
        [MaxLength(30, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el nombre del producto.")]
        public string Name { get; set; }

        /// <summary>
        /// Precio del Producto
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el precio del producto.")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Indica si el producto se le puede incluir agregados.
        /// </summary>
        public bool SupportAggregate { get; set; }

        /// <summary>
        /// Cantidad disponible en el Stock
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad de productos.")]
        public int Stock { get; set; }

        /// <summary>
        /// Descripción de la publicacion
        /// </summary>
        [Required(ErrorMessage = "Debe introducir una descripción del producto.")]
        [MaxLength(100, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para dar una descripción del producto.")]
        public string Description { get; set; }
    }
}
