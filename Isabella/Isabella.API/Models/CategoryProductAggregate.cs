namespace Isabella.API.Models
{
    using System.ComponentModel.DataAnnotations;
    using Extras;

    /// <summary>
    /// Entidad que representa la categoria de un producto del tipo agregado.
    /// </summary>
    public class CategoryProductAggregate : IModel
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
