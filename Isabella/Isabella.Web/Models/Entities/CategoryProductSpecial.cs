namespace Isabella.Web.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Extras;

    /// <summary>
    /// Entidad que representa la categoria de un producto especial
    /// </summary>
    public class CategoryProductSpecial : IModel
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
