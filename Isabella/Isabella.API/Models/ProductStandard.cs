namespace Isabella.API.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Producto Standard
    /// </summary>
    public class ProductStandard : Product
    {
        /// <summary>
        /// SubCategorias.
        /// </summary>
        public ICollection<SubCategory_ProductStandard> SubCategory_ProductStandards { get; set; }

        /// <summary>
        /// Imagenes para los productos standars. 
        /// </summary>
        public ICollection<ImageProductStandard> ImageProductStandards { get; set; }
    }
}
