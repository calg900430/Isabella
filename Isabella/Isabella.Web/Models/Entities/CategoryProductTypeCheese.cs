namespace Isabella.Web.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Extras;

    /// <summary>
    /// Entidad que representa la categoria de los tipos de queso para los productos especiales
    /// </summary>
    public class CategoryProductTypeCheese : IModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría del producto.
        /// </summary>
        public string Name { get; set; }
    }
}
