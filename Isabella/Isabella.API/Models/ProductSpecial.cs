namespace Isabella.API.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Productos Especiales(Pizza y Pastas)
    /// </summary>
    public class ProductSpecial : Product
    {
        
        /// <summary>
        /// SubCategorias.
        /// </summary>
        public ICollection<SubCategory_ProductSpecial> SubCategory_ProductSpecials { get; set; }

        /// <summary>
        /// Tipos de agregados.
        /// </summary>
        public ICollection<ImageProductSpecial> ImageProductSpecials { get; set; }
    }
}
