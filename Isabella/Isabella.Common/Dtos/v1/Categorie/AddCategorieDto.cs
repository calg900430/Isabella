namespace Isabella.Common.Dtos.Categorie
{
    using System.ComponentModel.DataAnnotations;

    public class AddCategorieDto
    {
        [Required(ErrorMessage = "Es necesario definir un nombre para la categoria.")]
        [MaxLength(20, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el nombre de una categoria.")]
        public string Name { get; set; }
    }
}
