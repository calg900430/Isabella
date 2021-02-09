namespace Isabella.Common.Dtos.SubCategory
{
    using System.ComponentModel.DataAnnotations;

    public class AddSubCategoryDto
    {
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
    }
}
