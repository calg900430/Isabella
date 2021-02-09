namespace Isabella.API.Models
{
    using Extras;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// SubCategoria de Producto Standard
    /// </summary>
    public class SubCategory_ProductStandard : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto Special
        /// </summary>
        public ProductStandard ProductStandard { get; set; }

        /// <summary>
        /// SubCategoria.
        /// </summary>
        public SubCategory SubCategory { get; set; }
    }
}
