namespace Isabella.Common.Dtos.Categorie
{
    using System.ComponentModel.DataAnnotations;

    public class AddCategorieDto
    {
        [Required(ErrorMessage = "Es necesario definir un nombre para la categoria.")]
        public string Name { get; set; }
    }
}
