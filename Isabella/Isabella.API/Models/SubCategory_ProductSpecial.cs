namespace Isabella.API.Models
{
   
    using System.ComponentModel.DataAnnotations;
    using Extras;

    /// <summary>
    /// SubCategoria de Productom Special.
    /// </summary>
    public class SubCategory_ProductSpecial : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto Special
        /// </summary>
        public ProductSpecial ProductSpecial { get; set; }

        /// <summary>
        /// SubCategoria.
        /// </summary>
        public SubCategory SubCategory { get; set; }
    }
}
