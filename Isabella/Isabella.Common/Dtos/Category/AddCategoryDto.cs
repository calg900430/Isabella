namespace Isabella.Common.Dtos.Category
{
    using System.ComponentModel.DataAnnotations;

    public class AddCategoryDto
    {
        [Required(ErrorMessage = "Es necesario definir un nombre para la categoria.")]
        public string Name { get; set; }
    }
}
