namespace Isabella.Web.Models.Entities
{
    using System.Collections.Generic;

    /// <summary>
    /// Producto Standard
    /// </summary>
    public class ProductStandard : Product
    {
        /// <summary>
        /// Categoria de los productos estandar.
        /// </summary>
        public CategoryProductStandard CategoryProductStandard { get; set; }

        /// <summary>
        /// Imagenes para los productos standars. 
        /// </summary>
        public ICollection<ImageProductStandard> ImageProductStandards { get; set; }
    }
}
