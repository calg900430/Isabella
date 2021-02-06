namespace Isabella.Common.Dtos.Product
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Extras;
    public class AddProductDto
    {
        /// <summary>
        /// Código de usuario
        /// </summary>
        [Required(ErrorMessage = "Debe introducir su código de usuario.")]
        [MaxLength(20, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos.")]
        public string CodeUser { get; set; }

        /// <summary>
        /// Nombre 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Producto
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool IsAvailabe { get; set; } = true;

        /// <summary>
        /// Cantidad disponible en el Stock
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Descripción de la publicacion
        /// </summary>
        [Required(ErrorMessage = "Introduzca una descripción del producto.")]
        public string Description { get; set; }

        /// <summary>
        /// Imagenes de publicaciones
        /// </summary>
        public List<byte[]> ImagesProduct { get; set; } = new List<byte[]>();

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        [Required(ErrorMessage = "Es necesario que agregue la categoria del producto.")]
        [Range(1, 6, ErrorMessage = "No está en el rango de la categoria de productos agregados. 1-Pastas, 8-Pizzas.")]
        public EnumCategories Categorie { get; set; }
    }
}
