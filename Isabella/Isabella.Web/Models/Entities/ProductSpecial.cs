namespace Isabella.Web.Models.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Productos Especiales(Pizza y Pastas)
    /// </summary>
    public class ProductSpecial : Product
    {
        /// <summary>
        /// Categoria de productos especiales.
        /// </summary>
        public CategoryProductSpecial CategoryProductSpecial { get; set; }

        /// <summary>
        /// Tipos de agregados.
        /// </summary>
        public ICollection<ImageProductSpecial> ImageProductSpecials { get; set; }
    }
}
