namespace Isabella.Common.Dtos.SubCategorie
{
    using System.ComponentModel.DataAnnotations;

    public class AddSubCategorieToProductDto
    {
        /// <summary>
        /// Producto.
        /// </summary>
        [Required(ErrorMessage = "Es necesario introducir el Id del producto al cual desea agregarle una nueva subcategoria.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        [Required(ErrorMessage = "Es necesario definir un nombre para la categoria.")]
        public string Name { get; set; }

        /// <summary>
        /// Precio. 
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el precio que define la subcategoria para el producto a la que se le aplique.")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Descripción
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para la descripción de una subcategoria.")]
        public string Description { get; set; }

        /// <summary>
        /// Enable
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
