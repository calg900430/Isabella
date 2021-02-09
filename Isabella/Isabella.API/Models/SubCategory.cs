namespace Isabella.API.Models
{
   
    using System.ComponentModel.DataAnnotations;
    using Extras;

    /// <summary>
    /// 
    /// </summary>
    public class SubCategory : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría del producto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }
    }
}
