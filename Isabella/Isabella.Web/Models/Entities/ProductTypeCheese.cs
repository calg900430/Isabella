namespace Isabella.Web.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Common.Extras;
    using Web.Extras;
   
    /// <summary>
    /// Entidad que representa los tipos de quesos que pueden utilizar los productos.
    /// cuando son Pizzas y Pastas.
    /// </summary>
    public class ProductTypeCheese : Product
    {
        /// <summary>
        /// Categorias para los tipos de queso
        /// </summary>
        public CategoryProductTypeCheese CategoryProductTypeCheese { get; set; }

        /// <summary>
        /// Imagenes para los tipos de queso. 
        /// </summary>
        public ICollection<ImageProductTypeCheese> ImageProductTypeCheeses { get; set; }
    }
}
