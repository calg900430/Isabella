namespace Isabella.Web.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Models;

    /// <summary>
    /// Entidad que representa la categoria de un producto standard
    /// </summary>
    public class Category : IEntity
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
